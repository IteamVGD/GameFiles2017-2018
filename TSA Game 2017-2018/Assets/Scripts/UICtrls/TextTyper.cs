using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextTyper : MonoBehaviour
{

    public float letterPause = 0.2f;

    public AudioClip typeSound1;
    public AudioClip typeSound2;
    public AudioClip typeSoundSpace;

    private AudioSource typeSound1Source;
    private AudioSource typeSound2Source;
    private AudioSource typeSoundSpaceSource;

    public string message;
    Text textComp;

    private void Awake()
    {
        typeSound1Source = AddAudio(typeSound1, false, false, 0.5f);
        typeSound2Source = AddAudio(typeSound2, false, false, 0.5f);
        typeSoundSpaceSource = AddAudio(typeSoundSpace, false, false, 0.5f);
    }

    // Use this for initialization
    void Start()
    {
        textComp = GetComponent<Text>();
        message = textComp.text;
        textComp.text = "";
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        foreach (char letter in message.ToCharArray())
        {
            textComp.text += letter;
            if(letter.ToString() == " ")
            {
                typeSoundSpaceSource.Play();
            }
            else
            {
                int randomInt = Random.Range(1, 3); //1 or 2
                if (randomInt == 1)
                    typeSound1Source.Play();
                else
                    typeSound2Source.Play();
            }
            yield return 0;
            yield return new WaitForSeconds(letterPause);
        }
    }

    public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.clip = clip;
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        return newAudio;
    }
}