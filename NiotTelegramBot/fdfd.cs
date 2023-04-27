public class Pipe : MonoBehaviour
{
    private Character character;

// Это нужно для инициализации    
    void Start()
    {
        character = FindObjectOfType();
    }

// Update вызывается один раз на кадр    
    void Update()
    {
        if (character.transform.position.x - transform.position.x > 30)
        {
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            character.Death();
        }
    }
}