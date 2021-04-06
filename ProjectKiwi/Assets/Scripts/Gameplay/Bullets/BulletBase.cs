using UnityEngine;

namespace Gameplay.Bullets
{
    public class BulletBase : MonoBehaviour
    {
        public BulletData data;
        public int damage;
        private float speed;
        [HideInInspector]
        public float lifeRange = 3f; 
    
        private Vector3 firedPosition;

        private void Awake()
        {
            speed = data.speed;
        }

        public void PoolOnInit()
        {
            firedPosition = transform.position;
        }

        public void PoolOnDestroy()
        {

        }

        private void Update()
        {
            transform.Translate(Vector3.forward * (speed * Time.deltaTime));
            if (Vector3.Distance(firedPosition, transform.position) > lifeRange) 
            {
                OnReachRangeLife();
            }
        }

        private void OnReachRangeLife()
        {
            if (!data.explodeWhenDie)
                return;

            Collider[] colInfo = Physics.OverlapSphere(transform.position, data.explosionRadius);

            if (colInfo != null)
            {
                foreach (Collider hit in colInfo)
                {
                    IHittableObject hitChar = hit.GetComponent<IHittableObject>();
                    if (hitChar != null)
                    {
                        //TODO: explosion affects enemies
                        //Rigidbody rb = hit.GetComponent<Rigidbody>();
                        //if (rb != null)
                        //    rb.AddExplosionForce(data.explosionPower, transform.position, data.explosionRadius, 3.0F);

                        hitChar.OnHit(damage);
                    }
                }
            }

            SpawnerController.instance.OnPoolingBullets(gameObject, this);
        }

        private void OnTriggerEnter(Collider collider)
        {
            IHittableObject hitObj = collider.GetComponent<IHittableObject>();

            if (hitObj != null)
            {
                hitObj.OnHit(damage);
                SpawnerController.instance.OnPoolingBullets(gameObject, this);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, data.explosionRadius);
        }
    }
}