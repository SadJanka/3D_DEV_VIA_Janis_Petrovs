using UnityEngine;
using TMPro;
using System.Collections;

public class ExitDoor : MonoBehaviour
{
    [Header("Iestatījumi")]
    public int vajadzigasAtslegas = 5;

    [Header("UI Elementi (Tikai mazais palīdzības teksts)")]
    public TextMeshProUGUI infoTeksts;      // Mazais teksts ekrāna vidū ("Durvis ir slēgtas")

    [Header("UI Ikonas virs durvīm (World Space)")]
    public GameObject eIcon;        // E_Icon no World Space Canvas
    public GameObject lockedIcon;   // Locked_Icon no World Space Canvas

    private bool speletajsIrPieDurvim = false;
    private PlayerMovement speletajaKustiba;
    private bool speleIrUzvareta = false;

    void Start()
    {
        if (eIcon != null) eIcon.SetActive(false);
        if (lockedIcon != null) lockedIcon.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (speleIrUzvareta) return;

        if (other.CompareTag("Player"))
        {
            speletajsIrPieDurvim = true;

            speletajaKustiba = other.GetComponent<PlayerMovement>();
            if (speletajaKustiba == null) speletajaKustiba = other.GetComponentInChildren<PlayerMovement>();
            if (speletajaKustiba == null) speletajaKustiba = other.GetComponentInParent<PlayerMovement>();

            AtjaunotDurvjuUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (speleIrUzvareta) return;

        if (other.CompareTag("Player"))
        {
            speletajsIrPieDurvim = false;
            speletajaKustiba = null;

            if (infoTeksts != null) infoTeksts.text = "";
            if (eIcon != null) eIcon.SetActive(false);
            if (lockedIcon != null) lockedIcon.SetActive(false);
        }
    }

    void Update()
    {
        if (speleIrUzvareta) return;

        if (speletajsIrPieDurvim)
        {
            if (speletajaKustiba == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null) speletajaKustiba = playerObj.GetComponent<PlayerMovement>();
            }

            if (speletajaKustiba != null)
            {
                AtjaunotDurvjuUI();

                int pasreizIr = speletajaKustiba.collectedCount;

                if (pasreizIr >= vajadzigasAtslegas && Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(UzvarasSecvence());
                }
            }
        }
    }

    void AtjaunotDurvjuUI()
    {
        if (speletajaKustiba == null) return;

        int pasreizIr = speletajaKustiba.collectedCount;

        if (pasreizIr >= vajadzigasAtslegas)
        {
            if (infoTeksts != null) infoTeksts.text = "Spied E, lai izietu un UZVARĒTU!";
            if (eIcon != null) eIcon.SetActive(true);
            if (lockedIcon != null) lockedIcon.SetActive(false);
        }
        else
        {
            int trukstAtslegas = vajadzigasAtslegas - pasreizIr;
            if (infoTeksts != null) infoTeksts.text = "Durvis ir slēgtas! Tev vajag vēl " + trukstAtslegas + " atslēgas.";
            if (eIcon != null) eIcon.SetActive(false);
            if (lockedIcon != null) lockedIcon.SetActive(true);
        }
    }

    IEnumerator UzvarasSecvence()
    {
        speleIrUzvareta = true;

        if (infoTeksts != null) infoTeksts.text = "";
        if (eIcon != null) eIcon.SetActive(false);
        if (lockedIcon != null) lockedIcon.SetActive(false);

        if (speletajaKustiba != null)
        {
            speletajaKustiba.enabled = false; // Aptur spēlētāju
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return new WaitForSeconds(10f); // Gaida 10 sekundes

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Izslēdz Unity Editorā
#else
        Application.Quit(); // Aizver gatavu spēli
#endif
    }

    void OnGUI()
    {
        if (speleIrUzvareta)
        {
            // Galvenā teksta stils (Balts)
            GUIStyle galvenaisStils = new GUIStyle();
            galvenaisStils.alignment = TextAnchor.MiddleCenter;
            galvenaisStils.fontSize = 85;
            galvenaisStils.fontStyle = FontStyle.Bold;
            galvenaisStils.normal.textColor = Color.white; // BALTA KRĀSA

            // Ēnas stils (Melns)
            GUIStyle enasStils = new GUIStyle(galvenaisStils);
            enasStils.normal.textColor = Color.black;

            Rect ekranaPozicija = new Rect(0, 0, Screen.width, Screen.height);

            // Zīmējam ēnu 
            GUI.Label(new Rect(ekranaPozicija.x + 4, ekranaPozicija.y + 4, ekranaPozicija.width, ekranaPozicija.height), "YOU WIN!", enasStils);

            // Pa virsu zīmējam balto tekstu
            GUI.Label(ekranaPozicija, "YOU WIN!", galvenaisStils);
        }
    }
}