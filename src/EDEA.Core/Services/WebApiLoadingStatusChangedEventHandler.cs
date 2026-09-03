using EDEA.Models;

namespace EDEA.Services;

/// <summary>Represents a method that handles the WebApiLoadingStatusChanged event.</summary>
/// <param name="sender">The source of the event.</param>
/// <param name="webApiParameter">The WebApiParameter? value of the webApiParameter parameter.</param>
public delegate void WebApiLoadingStatusChangedEventHandler(object? sender, WebApiParameter? webApiParameter);
