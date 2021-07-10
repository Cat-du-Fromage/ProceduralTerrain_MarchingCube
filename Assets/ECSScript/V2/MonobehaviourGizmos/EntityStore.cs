using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace KaizerWaldCode.V2
{
    public class EntityStore : MonoBehaviour
    {
        public Entity entity
        {
            get;
            set;
        }

        public Entity EventHolder
        {
            get;
            set;
        }
    }
}
