using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magic365.Client.ViewModels.Models;
using Magic365.Shared.DTOs;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace Magic365.WinUI.Converters;
public class AutoSuggestQueryParameterConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        // cast value to whatever EventArgs class you are expecting here
        var args = (AutoSuggestBoxQuerySubmittedEventArgs)value;
        // return what you need from the args
        return new AutoSuggestQueryRequest
        {
            IsSearchNeeded = args.ChosenSuggestion == null,
            SelectedPerson = args.ChosenSuggestion as MeetingPerson,
            Query = args.QueryText
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}