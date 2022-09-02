using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BasDefence
{
    internal class EnemyData
    {
        public float maxHealth;
        public float health;
        public float damage;
        public GameObject obj;

        internal EnemyData(GameObject obj)
        {
            maxHealth = 100f;
            health = maxHealth;
            damage = 5f;
            this.obj = obj;
        }

        internal void Damage(float damage)
        {
            health -= damage;
        }
    }
}
