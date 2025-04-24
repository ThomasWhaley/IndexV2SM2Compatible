using System.Windows.Media.Imaging;
using Index.UI.ViewModels;

namespace Index.Modules.TextureEditor.ViewModels
{

  public class TextureImageViewModel : ViewModelBase
  {

    public int ImageIndex { get; set; }
    public BitmapImage Preview { get; set; }

  }

}
