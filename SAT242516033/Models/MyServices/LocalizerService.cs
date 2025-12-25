using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Globalization;

namespace SAT242516033.Models.MyServices
{
    public class LocalizerService<T>
    {
        private readonly IStringLocalizer<T> _localizer;
        private readonly IJSRuntime _jsRuntime;

        public LocalizerService(IStringLocalizer<T> localizer, IJSRuntime jsRuntime)
        {
            _localizer = localizer;
            _jsRuntime = jsRuntime;
        }

        public string this[string key]
        {
            get
            {
                return _localizer[key];
            }
        }

        // --- İŞTE EKSİK OLAN METOD BURASI ---
        public async Task SetCulture(string cultureName)
        {
            if (cultureName != "tr" && cultureName != "en") cultureName = "tr";

            var culture = new CultureInfo(cultureName);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            // Tarayıcı çerezine yazalım ki sayfayı yenileyince hatırlasın
            await _jsRuntime.InvokeVoidAsync("blazorCulture.set", cultureName);
        }
    }
}