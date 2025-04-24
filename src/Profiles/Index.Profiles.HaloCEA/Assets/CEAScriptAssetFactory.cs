using Prism.Ioc;

namespace Index.Profiles.HaloCEA.Assets
{

  public class CEAScriptAssetFactory : CEACacheBlockEntryAssetFactory<CEAScriptAsset>
  {

    #region Constructor

    public CEAScriptAssetFactory( IContainerProvider container )
      : base( container )
    {
    }

    #endregion

  }

}
