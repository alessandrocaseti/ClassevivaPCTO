﻿using ClassevivaPCTO.Utils;
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ClassevivaPCTO.Converters
{
    public class AbsenceTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            SolidColorBrush brush = new SolidColorBrush();

            AbsenceEventCode valore = (AbsenceEventCode)value;


            switch (valore)
            {
                case AbsenceEventCode.ABA0:
                    brush.Color = Colors.Crimson;
                    break;

                case AbsenceEventCode.ABR0:
                    brush.Color = Colors.DarkOrange;
                    break;

                case AbsenceEventCode.ABR1:
                    brush.Color = Colors.DarkOrange;
                    break;

                case AbsenceEventCode.ABU0:
                    brush.Color = Colors.Goldenrod;
                    break;
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException();
        }
    }

}
