using System;

namespace HyperDigger
{
    public interface IDamageable
    {
        public void DoDamage(GameObject source, int damage);
    }
}
