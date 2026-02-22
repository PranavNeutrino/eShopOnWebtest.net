using System;
using BlazorShared.Models;

namespace BlazorShared.Interfaces;

public interface IToastService
{
    event Action<string, ToastLevel> OnShow;
    event Action OnHide;
    void ShowToast(string message, ToastLevel level);
}
