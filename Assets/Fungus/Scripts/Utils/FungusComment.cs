#if UNITY_LOCALIZATION
using System;
using UnityEngine.Localization.Metadata;

namespace Fungus
{
    [Metadata(AllowedTypes = MetadataType.AllSharedTableEntries)]
    [Serializable]
    public class FungusComment : IMetadata
    {
        public string CommentText;
    }
}
#endif