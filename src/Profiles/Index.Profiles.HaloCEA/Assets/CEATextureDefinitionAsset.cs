using Index.Domain.Assets;

namespace Index.Profiles.HaloCEA.Assets
{

  public class CEATextureDefinitionAsset : CEACacheBlockEntryAsset
  {

    public override string TypeName => "Texture Definition";

    public CEATextureDefinitionAsset( IAssetReference assetReference )
      : base( assetReference )
    {
    }

  }

}
