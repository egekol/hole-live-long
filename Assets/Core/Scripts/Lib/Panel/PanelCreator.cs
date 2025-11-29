using System.Threading.Tasks;
using Lib.CacheManager;
using Lib.Debugger;

namespace Lib.Panel
{
    public interface IPanelCreator
    {
        Task<T> CreatePanel<T>(string key) where T : class, IPanel;
    }

    public class PanelCreator : IPanelCreator
    {
        public IPanelCanvas PanelCanvas { get; set; }
        public IInstantiator Instantiator { get; set; }

        public PanelCreator(IPanelCanvas panelCanvas, IInstantiator instantiator)
        {
            PanelCanvas = panelCanvas;
            Instantiator = instantiator;
        }

        public async Task<T> CreatePanel<T>(string key) where T : class, IPanel
        {
            var popup = await Instantiator.InstantiateAsync<T>(key, PanelCanvas.Parent);

            if (popup == null)
            {
                LogHelper.LogError($"Failed to create panel with key: {key}", "PanelCreator");
                return null;
            }
            popup.Initialize(PanelCanvas);
            return popup;
        }
    }
}