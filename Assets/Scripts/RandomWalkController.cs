using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// basically only for use on the title screen to make things walk around
public class RandomWalkController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 3f;
    private Animator _animator;
    private bool _moving;

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
        transform.position = GetRandomLocation();
    }

    private void Update()
    {
        if (!_moving)
        {
            StartCoroutine(WalkTo(GetRandomLocation()));
        }
    }

    IEnumerator WalkTo(Vector3 destination)
    {
        _moving = true;
        _animator.SetBool("Moving", true);
        while (Vector3.Distance(transform.position, destination) >= 0.25f)
        {
            transform.position += _movementSpeed * Time.deltaTime * 
                (destination - transform.position).normalized;
                yield return null;
        }
        _animator.SetBool("Moving", false);
        yield return new WaitForSeconds(Random.Range(2, 5));
        _moving = false;
    }

    private Vector3 GetRandomLocation()
    {
        return new Vector3(Random.Range(-6f, 6f), Random.Range(-9.5f, 9.5f), 0f);
    }

}
