using UnityEngine;

namespace Gameplay.Bullets
{
    public class BulletBase : MonoBehaviour
    {
        public int damage;
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
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            ICharacter character = other.transform.GetComponent<ICharacter>();

            if (character != null)
            {
                character.OnHit(damage);
                SpawnerController.instance.OnPoolingBullets(gameObject, this);
            }
        }
    }
}