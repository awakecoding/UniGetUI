using System;
using Avalonia.Controls;
using Avalonia.Input;

namespace UniGetUI.Pages.PageInterfaces;
internal interface ISearchBoxPage
{
    public string QueryBackup { get; set; }
    public string SearchBoxPlaceholder { get; }
    public void SearchBox_TextChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs args);
    public void SearchBox_QuerySubmitted(object? sender, Avalonia.Interactivity.RoutedEventArgs args);
}
