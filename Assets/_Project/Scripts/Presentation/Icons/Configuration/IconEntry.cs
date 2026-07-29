using System;
using System.Collections.Generic;
using LL.Identifiers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Presentation.Icons.Configuration
{
    [Serializable]
    internal abstract class IconEntry<TId>
        where TId : struct, IIdentifier
    {
        [LabelText("ID")]
        [SerializeField] private string _id;

        [Required]
        [PreviewField(64, ObjectFieldAlignment.Center)]
        [SerializeField] private Sprite _icon;

        internal TId Id => CreateId(_id);

        internal KeyValuePair<TId, Sprite> ToPair() => new(Id, _icon);

        protected abstract TId CreateId(string value);
    }
}