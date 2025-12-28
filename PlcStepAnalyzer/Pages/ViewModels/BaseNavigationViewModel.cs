using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcStepAnalyzer.Pages.ViewModels
{
    public class BaseNavigationViewModel : BindableBase, INavigationAware
    {
        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            //
        }
    }
}
