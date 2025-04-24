using System.Windows.Input;

namespace Index.UI.ViewModels
{

  public abstract class TabViewModelBase : ViewModelBase, ITabViewModel
  {

    #region Properties

    public string TabName { get; set; }
    public ICommand CloseCommand { get; }

    #endregion

  }

}
