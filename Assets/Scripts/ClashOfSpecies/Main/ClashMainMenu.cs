﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class ClashMainMenu : MonoBehaviour
{
    private ClashGameManager manager;

    public VerticalLayoutGroup playerListGroup;
    public List<Player> playerList = new List<Player>();
    public Transform contentPanel;
    public GameObject playerItemPrefab;
    public Button attackBtn;
	private static int flag = 0; 
	public List<string> carnivore {get; set;}
	public List<string> omnivore {get; set;}
	public List<string> herbivore {get; set;}
	public List<string> plant {get; set;}

    private Player selectedPlayer = null;
    private ToggleGroup toggleGroup;

    void Awake()
    {
        manager = GameObject.Find("MainObject").GetComponent<ClashGameManager>();
		if (flag == 0) {
			flag = 1;
			ShowNotification ();
		}
		carnivore = new List<string> ();
		omnivore = new List<string> ();
		herbivore = new List<string> ();
		plant = new List<string> ();
    }

    void Start()
    {
        if (manager.currentPlayer.credits < 10)
        {
            attackBtn.interactable = false;
        }

        contentPanel.Find("Credit").GetComponent<Text>().text = "You have " + manager.currentPlayer.credits + " credits.";

        NetworkManagerCOS.getInstance().Send(ClashPlayerListProtocol.Prepare(), (res) =>
            {
                var response = res as ResponseClashPlayerList;

                foreach (var pair in response.players)
                {
                    Player player = new Player(pair.Key);
                    player.name = pair.Value;
                    playerList.Add(player);

                    var item = Instantiate(playerItemPrefab) as GameObject;
                    item.transform.SetParent(playerListGroup.transform);
                    item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 0.0f);
                    item.transform.localScale = Vector3.one;

                    item.GetComponentInChildren<Text>().text = player.name;

                    var toggle = item.GetComponentInChildren<Toggle>().group = playerListGroup.GetComponent<ToggleGroup>();
                    item.GetComponentInChildren<Toggle>().onValueChanged.AddListener((val) =>
                        {
                            contentPanel.Find("Message").GetComponent<Text>().enabled = !val;
                            if (val)
                            {
								carnivore.Clear();
								omnivore.Clear();
								herbivore.Clear();
								plant.Clear();
                                selectedPlayer = player;
                                manager.currentTarget = new ClashDefenseConfig();
                                NetworkManagerCOS.getInstance().Send(ClashPlayerViewProtocol.Prepare(player.GetID()), (resView) =>
                                    {
                                        var responseView = resView as ResponseClashPlayerView;
//                            Debug.Log(responseView.terrain);
                                        contentPanel.GetComponent<RawImage>().texture = Resources.Load("Images/ClashOfSpecies/" + responseView.terrain) as Texture;
                                        manager.currentTarget.owner = player;
                                        manager.currentTarget.terrain = responseView.terrain;
                                        manager.currentTarget.layout = responseView.layout.Select(x =>
                                            {
                                                var species = manager.availableSpecies.Single(s => s.id == x.Key);
												SaveSpecies(species.name.ToString(), species.type.ToString());
                                                var positions = x.Value;
                                                return new { 
                                    species,
                                    positions
                                };
                                            }).ToDictionary(p => p.species, p => p.positions);
                                    });
                            }
                            else
                            {
                                selectedPlayer = null;
                                contentPanel.GetComponent<RawImage>().texture = null;
                            }
                        });
                }
            });
    }

	public void SaveSpecies (string speciesName, string speciesType){
		var name = speciesName;
		var type = speciesType;

		if (type == "CARNIVORE") {
			carnivore.Add (speciesName);
		}else if (type == "OMNIVORE") {
			omnivore.Add (speciesName);
		}else if (type == "HERBIVORE") {
			herbivore.Add (speciesName);
		}else if (type == "PLANT") {
			plant.Add (speciesName);
		}
	}

    void Update()
    {
    }

    public void ReturnToLobby()
    {
        Game.SwitchScene("World");
    }

    public void EditDefense()
    {
        Game.LoadScene("ClashDefenseShop");
    }

    public void Attack()
    {
        if (manager.currentTarget != null && manager.currentTarget.owner != null)
        {
            Game.LoadScene("ClashAttackShop");
        }
    }

    public void GoToInputSetup()
    {
        Game.LoadScene("InputTestScene");
    }

	public void ShowPlayerHistory(){

		gameObject.AddComponent <PlayerHistoryGUI>();
	}

	public void ShowLeaderboard(){

		gameObject.AddComponent <ClashLeaderboardGUI>();
	}

	public void ShowNotification(){

		gameObject.AddComponent <ClashNotificationGUI>();
	}

	public void ShowHints(){

		gameObject.AddComponent <ClashHintsGUI>();
	}
}
