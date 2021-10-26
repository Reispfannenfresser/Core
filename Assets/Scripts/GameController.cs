using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

public class GameController : MonoBehaviour {
	[SerializeField]
	SegmentPlacer segment_placer = null;
	[SerializeField]
	GameObject start_segment = null;
	[SerializeField]
	GameObject[] enemy_types = new GameObject[0];
	[SerializeField]
	GameObject[] boss_types = new GameObject[0];
	[SerializeField]
	Image[] ui_images = new Image[0];

	[SerializeField]
	GameObject crawler = null;

	[SerializeField]
	Text zollar_text = null;
	[SerializeField]
	Text zollar_change_text = null;
	[SerializeField]
	Text enemy_count_text = null;
	[SerializeField]
	Text round_count_text = null;
	[SerializeField]
	Text restart_text = null;
	[SerializeField]
	Text highscore_text = null;
	[SerializeField]
	Button resume_button = null;
	[SerializeField]
	Button next_wave_button = null;

	public AudioMixer mixer;

	private AudioSource test_sound = null;
	bool volume_changed = false;

	float current_ui_scale = 1;
	float next_ui_scale = 1;
	bool ui_scale_changed = false;

	[SerializeField]
	GameObject hud = null;
	[SerializeField]
	GameObject menu = null;

	int bosses_at = 10;
	int current_wave = 0;
	int zollars = 0;
	int zollar_change = 0;
	public bool is_paused = true;
	public bool is_started = true;
	public bool is_practice = false;

	[SerializeField]
	private Color ui_color = Color.white;
	[SerializeField]
	private Color ui_selected_color = Color.white;

	[SerializeField]
	GameObject balance_change_indicator = null;

	int highscore = 0;

	int spawn_amount = 0;
	int boss_spawn_amount = 0;

	public HashSet<GameObject> delete_on_start = new HashSet<GameObject>();

	public static GameController instance = null;

	void Awake() {
		instance = this;
	}

	void Start() {
		SetPaused(true);
		is_started = false;
		instance.SetSegment(start_segment);
		instance.ResetUIImageColors(0);
		test_sound = GetComponent<AudioSource>();
	}

	public void ChangeMusicVolume(float sliderValue) {
		mixer.SetFloat("Music", Mathf.Log10(sliderValue) * 20);
	}

	public void ChangeSfxVolume(float sliderValue) {
		mixer.SetFloat("Sfx", Mathf.Log10(sliderValue) * 20);
		volume_changed = true;
	}

	public void ChangeUIScale(float scale) {
		next_ui_scale = scale;
		ui_scale_changed = true;
	}

	public void ResumeGame() {
		SetPaused(false);
	}

	public void StartGame() {
		foreach(GameObject to_delete in delete_on_start) {
			if (to_delete != null) {
				Destroy(to_delete);
			}
		}

		current_wave = 0;

		ResetZollars();
		zollar_change = 0;

		spawn_amount = 0;
		boss_spawn_amount = 0;

		is_started = true;
		is_practice = false;
		SetPaused(false);

		restart_text.text = "Restart";
		resume_button.interactable = true;
		round_count_text.text = "" + current_wave;

		Instantiate(crawler, transform.position, transform.rotation);
	}

	public void StartPractice() {
		StartGame();
		restart_text.text = "Start";
		is_practice = true;
	}

	public void LoseGame() {
		resume_button.interactable = false;
		SetPaused(true);
		is_started = false;
	}

	public void QuitGame() {
		Application.Quit();
	}

	private void Update() {
		if (Input.GetButtonDown("Cancel")) {
			SetPaused(!is_paused);
		}

		bool ui_input = Input.GetMouseButtonUp(0) || Input.GetButtonUp("Horizontal");

		if(volume_changed && ui_input) {
			volume_changed = false;
			test_sound.Play();
		}
		if(ui_scale_changed && ui_input) {
			ui_scale_changed = false;
			float scale_factor = next_ui_scale / current_ui_scale;

			foreach (RectTransform rt in UnityEngine.Object.FindObjectsOfType<RectTransform>(true)) {
				rt.sizeDelta *= scale_factor;
			}

			current_ui_scale = next_ui_scale;
		}
	}

	public void ResetUIImageColors(int index) {
		foreach (Image image in ui_images) {
			image.color = ui_color;
		}
		ui_images[index].color = ui_selected_color;
	}

	public void SetPaused(bool paused) {
		if (!paused && !is_started) {
			StartGame();
			return;
		}
		is_paused = paused;
		Time.timeScale = is_paused ? 0 : 1;
		hud.SetActive(!paused);
		menu.SetActive(paused);
	}

	public void FixedUpdate() {
		if (spawn_amount > 0) {
			int index = UnityEngine.Random.Range(0, Math.Min(enemy_types.Length, spawn_amount));
			SpawnEnemy(enemy_types[index]);
			spawn_amount -= index + 1;
		}else if (boss_spawn_amount > 0) {
			int index = UnityEngine.Random.Range(0, Math.Min(boss_types.Length, boss_spawn_amount));
			SpawnEnemy(boss_types[index]);
			boss_spawn_amount -= index + 1;
		}

		enemy_count_text.text = "" + Enemy.total_enemies;

		bool round_continues = Enemy.harmful_enemies > 0;

		enemy_count_text.gameObject.SetActive(round_continues);
		next_wave_button.gameObject.SetActive(!round_continues);
		next_wave_button.interactable = !round_continues;
	}

	public void NextWave() {
		zollar_change = 0;
		zollar_change_text.text = "+" + zollar_change;

		if (is_practice) {
			return;
		}

		current_wave += 1;
		spawn_amount += current_wave * 2 + 5;

		if ((current_wave) % bosses_at == 0) {
			boss_spawn_amount += (current_wave) / bosses_at;
		}
		else if (current_wave >= 25) {
			boss_spawn_amount += (int) (UnityEngine.Random.value * current_wave / bosses_at);
		}

		round_count_text.text = "" + current_wave;
		if (highscore < current_wave) {
			highscore = current_wave;
			highscore_text.text = "" + highscore;
		}
	}

	private void SpawnEnemy(GameObject enemy) {
		Instantiate(enemy, transform.position, transform.rotation);
	}

	public void SetSegment(GameObject to_place) {
		segment_placer.SetSegment(to_place);
	}

	public int GetZollars() {
		return zollars;
	}

	public void ResetZollars() {
		ZollarsChanged(-zollars);
	}

	public void AddZollars(int amount) {
		ZollarsChanged(amount);
	}

	public void RemoveZollars(int amount) {
		ZollarsChanged(-amount);
	}

	private void ZollarsChanged(int change) {
		if (change == 0) {
			return;
		}

		zollar_change += change;
		zollar_change_text.text = "" + zollar_change;
		if (zollar_change >= 0) {
			zollar_change_text.text = "+" + zollar_change_text.text;
		}

		Graphic[] change_graphics = zollar_change_text.GetComponentsInChildren<Graphic>();
		foreach (Graphic g in change_graphics) {
			g.color = (zollar_change < 0) ? new Color(1, 0.5f, 0.5f) : new Color(0.5f, 1, 0.5f);
		}

		zollars += change;
		zollar_text.text = "" + zollars;

		GameObject new_balance_change = Instantiate(balance_change_indicator, zollar_change_text.transform);

		RectTransform[] transforms = new_balance_change.GetComponentsInChildren<RectTransform>();
		foreach (RectTransform t in transforms) {
			t.sizeDelta *= current_ui_scale;
		}

		Graphic[] graphics = new_balance_change.GetComponentsInChildren<Graphic>();
		foreach (Graphic g in graphics) {
			g.color = (change < 0) ? new Color(1, 0.5f, 0.5f) : new Color(0.5f, 1, 0.5f);
		}

		Text text = new_balance_change.GetComponent<Text>();
		text.text = "" + change;
		if (change > 0) {
			text.text = "+" + text.text;
		}
	}
}
