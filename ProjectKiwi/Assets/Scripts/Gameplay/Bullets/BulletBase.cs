using UnityEngine;

namespace Gameplay.Bullets
{
    public class BulletBase : MonoBehaviour
    {
        public float speed;
        public float lifeRange = 3f; 
    
        private Vector3 firedPosition;

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
                SpawnerController.instance.OnPoolingBullets(gameObject, this);
                // Destroy(gameObject);
            }
        }

        // private void OnCollisionEnter(Collision other)
        // {
        //     if (other.gameObject.CompareTag("Enemy"))
        //     {
        //         other.gameObject.GetComponent<Health>().takeDamage(2);
        //         Destroy(gameObject);
        //     }
        // }
    }
}