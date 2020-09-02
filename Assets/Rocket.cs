// using System;
// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Rocket : MonoBehaviour {

    [SerializeField] float rcsThrust = 400f;
    [SerializeField] float mainThrust = 3f;
    [SerializeField] float levelLoadDelay = 1f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody rigidBody;
    AudioSource audio;

    enum State {Alive, Dying, Transcending}
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            ThrustAndAudio();
            Steer();
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        if (state != State.Alive) {return;}

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                print("ok");
                break;
            case "Finish":
                SuccessSequence();
                break;
            default:
                DeathSequence();
                break;

        }
    }

    private void DeathSequence()
    {
        state = State.Dying;
        audio.Stop();
        audio.PlayOneShot(death);
        deathParticles.Play();
        Invoke("RestartGame", levelLoadDelay);
    }

    private void SuccessSequence()
    {
        state = State.Transcending;
        audio.Stop();
        audio.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay); // parameterise time
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(0);
        print("dead");
    }

    private void LoadNextScene()
    {
        print("finish");
        SceneManager.LoadScene(1); // todo allow more then two levels
    }

    private void Steer()
    {
        rigidBody.freezeRotation = true; //take manual control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false; //resume physics control of rotation

    }

    private void ThrustAndAudio()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audio.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime); // can thrust while rotating
        if (!audio.isPlaying)
        {
            audio.PlayOneShot(mainEngine);
            mainEngineParticles.Play();
        }
    }
}
