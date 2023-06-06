﻿using Polly;
using Polly.Wrap;
using Refit;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace ClassevivaPCTO.Utils
{
    //the app crashed with the error "Access is denied" because that class wasn't marked as "public"
    public class PoliciesDispatchProxy<T> : DispatchProxy
            where T : class, IClassevivaAPI
    {
        private T Target { get; set; }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            int retryCount = 3;
            int currentRetryAttempts = 0;

            var retryPolicy = Policy
                .Handle<AggregateException>()
                .RetryAsync(
                    retryCount,
                    async (exception, retryCount, context) =>
                    {
                        currentRetryAttempts = retryCount;

                        //we check whether the exception thrown is actually a Refit's ApiException
                        if (exception.InnerException is ApiException apiException)
                        {
                            if (
                                apiException.StatusCode is System.Net.HttpStatusCode.NotFound or System.Net.HttpStatusCode.Unauthorized
                            )
                            {
                                //TODO: refresh token
                                //await apiClient.RefreshTokenAsync();

                                Debug.WriteLine("Test retry n.{0} policy ok ", retryCount);


                            }
                        }
                    }
                );

            var fallback = Policy<object>
                .Handle<Exception>()
                .FallbackAsync(fallbackAction: async (ct) =>
                    {
                        
                    //if after the retries another exception occurs, then we let the call flow go ahead
                    return targetMethod.Invoke(Target, args);
                },
            onFallbackAsync: async (delegateResult) =>
            {

                TaskCompletionSource<bool> isSomethingLoading =
                    new TaskCompletionSource<bool>();

                //the dispatcher.runasync method does not return any value, so actually the "await" is redundant, so to know when the dialog is done showing, we use the Taskcompletionsource hack
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        ContentDialog noWifiDialog = new ContentDialog
                        {
                            Title = "Errore chiamata API",
                            Content =
                                "Tentativi effettuati: "
                                + currentRetryAttempts
                                + "\n"
                                + delegateResult.Exception.Message,
                            CloseButtonText = "Ok"
                        };

                        ContentDialogResult result =
                            await noWifiDialog.ShowAsync();

                        isSomethingLoading.SetResult(true);
                    }
                );

                await isSomethingLoading.Task;

                // Access the exception that triggered the fallback
                Debug.WriteLine($"Fallback triggered by exception: {delegateResult.Exception.Message}");
            });

            AsyncPolicyWrap<object> combinedpolicy = fallback.WrapAsync(retryPolicy);

            return combinedpolicy
                .ExecuteAsync(async () =>
                {
                    var result = (targetMethod.Invoke(Target, args));

                    if (result is Task task)
                    {
                        task.Wait(); //we wait for the result of the task, so we catch the exceptions if there are any
                    }

                    return result; //if no exception occur then we return the result of the method call
                })
                .Result;
        }

        public static T CreateProxy(T target)
        {
            var proxy = Create<T, PoliciesDispatchProxy<T>>() as PoliciesDispatchProxy<T>;
            proxy.Target = target;
            return proxy as T;
        }
    }
}
