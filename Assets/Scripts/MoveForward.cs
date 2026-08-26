using UnityEngine;

public class MoveForward : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody2D effetcRb;
    public float speed = 5f;
    private Animator anim;

    public float animspeed = 1;

    void Start()
    {
        effetcRb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        effetcRb.linearVelocity = new Vector2(transform.localScale.x * speed, 0);
        anim.speed = animspeed;
    }

    // Update is called once per frame
    void Update()
    {
       

    }
}
