using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Entity : MonoBehaviour
{
    // Components
    protected Animator anim;
    protected Item activeItem;
    protected EntityController controller;
    protected SpriteRenderer sRend;
    protected Collider2D hurtbox;

    // Combat stats
    public StatInfo stats;
	// Inspector editable stats
	public string[] defaultItemPrefabNames;
    public float MAX_HEALTH = 10;
    public float speed = 1f;
    public float attack = 1f;
    public float defense = 1f;

    private int collisionLayerMask;

    // State (used for animation also) 
    public enum CharacterState
    {
        Dead,
        Immobile,
        Still,
        Walking,
        Attacking,
        Blocking,
        Interacting
    }

	// Internal properties
    protected float ATTACK_SPEED_FACTOR = .5f;
    protected float BLOCK_SPEED_FACTOR = 0f;
    protected float RUN_SPEED_FACTOR = 2f;
    protected float NORMAL_SPEED_FACTOR = 1f;
    protected float SLOW_SPEED_FACTOR = .33f;

    protected float speedFactor;
    protected int primaryDir;
    protected int secondaryDir;
    protected float stunTimer;
    protected Vector3 stunVelocity;
    // Character alternates step animation; this toggles which one
    protected bool oddStep;

    // Convention used for directions, but not using the enum for simplicity
    public static Vector2[] directionVectors =
    {
        new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0)
    };

    public static float secondaryDirFactor = .5f;

    public enum Dir
    {
        Up = 1,
        Right,
        Down,
        Left
    }

    public virtual void Setup()
    {
        anim = GetComponent<Animator>();
        sRend = GetComponent<SpriteRenderer>();
		foreach (Collider2D cols in GetComponentsInChildren<Collider2D>()) {
			if (cols.name == "Collidable") {
				hurtbox = cols;
				break;
			}
		}

        stunTimer = 0;
        anim.SetInteger("state", (int) CharacterState.Still);
        anim.SetInteger("dir", (int) Dir.Down);
        oddStep = false;
        speedFactor = NORMAL_SPEED_FACTOR;
        stats = new StatInfo(new Dictionary<string, float>
        {
            {"health", MAX_HEALTH},
            {"attack", attack},
            {"defense", defense},
            {"speed", speed}
        });

        collisionLayerMask = LayerMask.GetMask("Wall", "Character", "Bedrock", "InteractiveTile");
    }

    public virtual void DoFixedUpdate()
    {
        //print (gameObject.name + "  " + (CharacterState)anim.GetInteger ("state"));
        //print (gameObject.name + "  " + (CharacterState)anim.GetInteger ("state") + "  " + anim.GetInteger ("dir") + "  " + speedFactor);

        // Timer updates
        if (stunTimer > 0 && DecrementTimer(stunTimer, out stunTimer))
        {
            sRend.color = new Color(1, 1, 1, 1);
            stunTimer = 0;
            anim.SetInteger("state", (int) CharacterState.Still);
        }

        // State-based updates
        switch (anim.GetInteger("state"))
        {
            case (int) CharacterState.Immobile:
            case (int) CharacterState.Walking:
                ExecuteWalk();
                break;
            case (int) CharacterState.Attacking:
            case (int) CharacterState.Blocking:
                if (primaryDir > 0)
                    ExecuteWalk();
                break;
            default:
                //print (gameObject.name + "  " + new Vector2 (0, 0));
                break;
        }
    }

    protected bool DecrementTimer(float value, out float timer)
    {
        timer = value -= Time.fixedDeltaTime;
        return timer <= 0;
    }

    private void ExecuteWalk()
    {
        Vector3 move;
        // If stunned, just continue launching with stunVelocity
        if (stunTimer > 0)
        {
            move = stunVelocity * Time.fixedDeltaTime;
        }
        else
        {
            // Calculate direction vector
            var dir = primaryDir - 1;
            if (dir < 0)
                dir = GetDirection() - 1;
            Vector3 dirVector = directionVectors[dir];
            if (secondaryDir > 0)
                dirVector =
                    Vector3.Normalize(dirVector + secondaryDirFactor * (Vector3) directionVectors[secondaryDir - 1]);
            move = speedFactor * speed * Time.fixedDeltaTime * dirVector;
        }

        var moveX = new Vector3(move.x, 0, 0);
        var moveY = new Vector3(0, move.y, 0);
        move = new Vector3(0, 0, 0);
        // BAD COLLISION CODE. WE NEED TO RESTRUCTURE COLLISIONS ENTIRELY. MORE TO COME
        // Collision(transform.position, (Vector2)move, move.magnitude + characterRadius, 1 << LayerMask.NameToLayer ("Wall"));
		var hits = Physics2D.BoxCastAll(transform.position + moveX, .5f*hurtbox.bounds.size, 0.0f, moveX, 0, collisionLayerMask);
        var xHit = false;
        for (var i = 0; i < hits.Length; i++)
        {
            // If didn't hit self
            var possibleHit = hits[i].collider.GetComponentInParent<Entity>();
            if (possibleHit != this)
            {
                xHit = true;
                break;
            }
        }
        if (!xHit) transform.Translate(moveX);
        // Collision(transform.position, (Vector2)move, move.magnitude + characterRadius, 1 << LayerMask.NameToLayer ("Wall"));
		hits = Physics2D.BoxCastAll(transform.position + moveY, .5f*hurtbox.bounds.size, 0.0f, moveY, 0, collisionLayerMask);
        //hits = Physics2D.BoxCastAll(transform.position, hurtbox.bounds.size, 0.0f, moveY, moveY.magnitude, collisionLayerMask);
        var yHit = false;
        for (var i = 0; i < hits.Length; i++)
        {
            var possibleHit = hits[i].collider.GetComponentInParent<Entity>();
            if (possibleHit != this)
            {
                yHit = true;
                break;
            }
        }
        if (!yHit) transform.Translate(moveY);
    }

    public void Quicken(bool active)
    {
        if (!CanActOutOfMovement())
            return;
        if (active) speedFactor = RUN_SPEED_FACTOR;
        else if (Mathf.Abs(speedFactor - RUN_SPEED_FACTOR) < .001) speedFactor = NORMAL_SPEED_FACTOR;
    }

    public void Slowen(bool active)
    {
        if (!CanActOutOfMovement())
            return;
        if (active) speedFactor = SLOW_SPEED_FACTOR;
        else if (Mathf.Abs(speedFactor - SLOW_SPEED_FACTOR) < .001) speedFactor = NORMAL_SPEED_FACTOR;
    }

    public virtual void EndWeaponUseAnim()
    {
        anim.SetInteger("state", (int) CharacterState.Still);
        speedFactor = NORMAL_SPEED_FACTOR;
    }

    public virtual void EndMovementAnim()
    {
        if (!CanActOutOfMovement())
            return;
        anim.SetInteger("state", (int) CharacterState.Still);
    }

    public bool CanActOutOfMovement()
    {
        if (GetState() > CharacterState.Walking || GetState() == CharacterState.Immobile) return false;
        if (stunTimer > 0)
            return false;
        return true;
    }

    public bool InAttack()
    {
        return GetState() == CharacterState.Attacking || GetState() == CharacterState.Blocking;
    }

    public bool CanSwitchFrom()
    {
        return GetState() <= CharacterState.Still;
    }

    // Sets animation state for walking
    public virtual void TryWalk()
    {
        if (GetState() != CharacterState.Still)
            return;
        // Alternate the step this walk cycle executes with
        oddStep = !oddStep;
        anim.SetTime(0);
        anim.SetInteger("state", (int) CharacterState.Walking);
        anim.SetBool("oddStep", oddStep);
        speedFactor = NORMAL_SPEED_FACTOR;
    }

    // Sets animation state for attacking
    public virtual void TryAttacking()
    {
        if (!CanActOutOfMovement() || !ActiveItemOfType(typeof(Weapon)))
            return;
        anim.SetTime(0);
        anim.SetInteger("state", (int) CharacterState.Attacking);
        ((Weapon) activeItem).StartAttack(GetDirection());
        speedFactor = ATTACK_SPEED_FACTOR;
    }

    public virtual void TryBlocking()
    {
        if (!CanActOutOfMovement() || !ActiveItemOfType(typeof(Weapon)))
            return;
        anim.SetTime(0);
        anim.SetInteger("state", (int) CharacterState.Blocking);
        ((Weapon) activeItem).StartBlock(GetDirection());
        speedFactor = BLOCK_SPEED_FACTOR;
    }

    public CharacterState GetState()
    {
        return (CharacterState) anim.GetInteger("state");
    }

    public float GetEntityStat(string statName)
    {
        return stats.GetStat(statName);
    }

    protected bool ActiveItemOfType<T>(T typeOfObj)
    {
        return activeItem != null && activeItem.GetType().Equals(typeOfObj);
    }

    public int GetDirection()
    {
        return anim.GetInteger("dir");
    }

    // Damages character, returns true if character is at 0 health
    public void Damage(float amount, int dirFrom, float stunTime, float stunVel)
    {
        stats.ChangeStat("health", -amount);
        controller.RespondToEntityAction(this, "health");
        if (stats.GetStat("health") <= 0)
        {
            Kill();
        }
        else
        {
            anim.SetInteger("state", (int) CharacterState.Immobile);
            stunTimer = stunTime;
            stunVelocity = stunVel * directionVectors[dirFrom - 1];
            sRend.color = new Color(1, 0, 0, 1);
        }
    }

    public void Kill()
    {
        anim.SetInteger("state", (int) CharacterState.Dead);
        controller.RespondToEntityAction(this, "die");
        foreach (var col in GetComponents<Collider2D>()) col.enabled = false;
    }
}