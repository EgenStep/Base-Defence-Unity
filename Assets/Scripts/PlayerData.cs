using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BasDefence
{
    internal class PlayerData
    {
        public float maxHealth;
        public float health;
        public float damage;
        public float fireN;

        internal PlayerData()
        {
            maxHealth = 100f;
            health = maxHealth;
            damage = 10f;
            fireN = 5f;
        }


    }
}
