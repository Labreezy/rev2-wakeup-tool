using System.Configuration;
using GGXrdWakeupDPUtil.Library;

namespace GGXrdWakeupDPUtil.ViewModels
{
    public class ScenarioWindowViewModel : ViewModelBase
    {
        
        
        
        public string Title => $"GGXrd Rev 2 Reversal Tool v{ConfigurationManager.AppSettings.Get("CurrentVersion")}";
        
        public bool AutoUpdate
        {
            get => ConfigurationManager.AppSettings.Get("AutoUpdate") == "1";
            set
            {
                ConfigManager.Set("AutoUpdate", value ? "1" : "0");


                this.OnPropertyChanged();
            }
        }
    }
}