using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour {

    private float moveSpeed = 0.5f;
    public Animator anim;
    private SpriteRenderer spriteRenderer;


    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {

        Movement();
	}




    void Movement()
    {
        //Controls movement and idle animations
        if (Input.GetKey("w"))
        {
            anim.SetBool("up", true);
            anim.SetBool("down", false);
            anim.SetBool("left", false);
            anim.SetBool("right", false);
            transform.Translate((Vector3.forward) * moveSpeed * Time.deltaTime);

        }
        if (Input.GetKey("s"))
        {
            anim.SetBool("up", false);
            anim.SetBool("down", true);
            anim.SetBool("left", false);
            anim.SetBool("right", false);
            transform.Translate((Vector3.back) * moveSpeed * Time.deltaTime);

        }
        if (Input.GetKey("a"))
        {
            //Flips the sprite back to normal 
            if(spriteRenderer.flipX == true)
            {
                spriteRenderer.flipX = false;
            }

            anim.SetBool("up", false);
            anim.SetBool("down", false);
            anim.SetBool("left", true);
            anim.SetBool("right", false);
            transform.Translate((Vector3.left) * moveSpeed * Time.deltaTime);

        }
        if (Input.GetKey("d"))
        {
            //flips the sprite to look as if it is walking left
            spriteRenderer.flipX = true;

            anim.SetBool("up", false);
            anim.SetBool("down", false);
            anim.SetBool("left", false);
            anim.SetBool("right", true);
            transform.Translate((Vector3.right) * moveSpeed * Time.deltaTime);

        }


        //This is what makes the walk animation
        if(Input.GetKey(KeyCode.W))
            anim.SetBool("walkUp", true);
        else
            anim.SetBool("walkUp", false);

        if (Input.GetKey(KeyCode.S))
            anim.SetBool("walkDown", true);
        else
            anim.SetBool("walkDown", false);

        if (Input.GetKey(KeyCode.A))
            anim.SetBool("walkLeft", true);
        else
            anim.SetBool("walkLeft", false);

        if (Input.GetKey(KeyCode.D))
            anim.SetBool("walkRight", true);
        else
            anim.SetBool("walkRight", false);


    }


}

