using System;
using Cysharp.Threading.Tasks;
using Lib.Tasks;

namespace Lib.Panel
{
    public class PanelTask<TPanel> : BaseTask where TPanel : class, IPanel
    {
        private readonly string _key;
        private readonly IPanelCreator _panelCreator;
        private readonly IPanelData _data;
        private TPanel _panel;
        private UniTaskCompletionSource _tcsPanel;

        private const string TaskName = "PanelTask: ";

        public PanelTask(string key, IPanelCreator panelCreator, IPanelData data)
        {
            _tcsPanel = new UniTaskCompletionSource();
            _key = key;
            _panelCreator = panelCreator;
            _data = data;
            SetName(TaskName + key);
        }
        
        public override async UniTask<bool> ExecuteAsync()
        {
            _panel = await _panelCreator.CreatePanel<TPanel>(_key);
            if (_panel == null)
            {
                _tcsPanel.TrySetException(new NullReferenceException("Panel is null. " + Name));
                return false;
            }
            _panel.SetData(_data);
            
            _tcsPanel.TrySetResult();
            await _panel.Show();
            await _panel.GetHideAwaiter();
            Complete();
            return true;
        }

        public TPanel GetPanel()
        {
            return _panel;
        }
        public UniTask GetPanelAwaiter()
        {
            return _tcsPanel.Task;
        }
    }
}