using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
  private bool menuActive = false;
  private bool helpActive = true;

  private GameObject player;
  private GameObject menu;
  private GameObject helper;

  void Awake() {
      player = GameObject.FindGameObjectWithTag("Player");
      menu = GameObject.FindGameObjectWithTag("Menu");
      helper = GameObject.FindGameObjectWithTag("Helper");
      menu.SetActive(false);

      helper.SetActive(true);
      Cursor.visible = true;
      player.GetComponent<PlayerController>().enabled = false;
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetButtonDown("Toggle Menu"))
    {
      menuActive = !menuActive;

      if (menuActive)
      {
        Cursor.visible = true;
        player.GetComponent<PlayerController>().enabled = false;

        helpActive = false;
        helper.SetActive(false);
      }
      else
      {
        Cursor.visible = false;
        player.GetComponent<PlayerController>().enabled = true;
      }

      menu.SetActive(menuActive);
    }

    if (Input.GetButtonDown("Toggle Help")) {
        helpActive = !helpActive;

        if (helpActive) {
            helper.SetActive(true);
            player.GetComponent<PlayerController>().enabled = false;
            Cursor.visible = true;

            menuActive = false;
            menu.SetActive(false);
        } else {
            helper.SetActive(false);
            player.GetComponent<PlayerController>().enabled = true;
            Cursor.visible = false;
        }
    }
  }
}
