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
	GameObject mothership = null;
	[SerializeField]
	Image[] ui_images = new Image[0];

	[SerializeField]
	GameObject crawler = null;

	GameObject core = null;
	public CoreSegment core_segment = null;

	[SerializeField]
	Text zollar_text = null;
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
	public bool is_paused = true;
	public bool is_started = true;
	public bool is_practice = false;

	public int boss_count = 0;

	[SerializeField]
	private Color ui_color = Color.white;
	[SerializeField]
	private Color ui_selected_color = Color.white;

	[SerializeField]
	GameObject balance_change_indicator = null;

	int highscore = 0;

	int budget = 0;
	int spawn_amount = 0;
	public int num_bosses = 0;

	public static GameController instance = null;

	public HashSet<Enemy> enemies = new HashSet<Enemy>();
	public int harmful_enemies = 0;
	public HashSet<ConstructionSegment> segments = new HashSet<ConstructionSegment>();
	public HashSet<Shot> shots = new HashSet<Shot>();
	public HashSet<Bomb> bombs = new HashSet<Bomb>();

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
		Destroy(core);

		foreach(Enemy enemy in enemies) {
			if (enemy != null) {
				enemy.Delete();
			}
		}

		foreach(ConstructionSegment segment in segments) {
			if (segment != null) {
				segment.Destroy();
			}
		}

		foreach(Shot shot in shots) {
			if (shot != null) {
				Destroy(shot.gameObject);
			}
		}

		foreach(Bomb bomb in bombs) {
			if (bomb != null) {
				Destroy(bomb.gameObject);
			}
		}

		current_wave = 0;
		ResetZollars();
		boss_count = 0;
		num_bosses = 0;
		spawn_amount = 0;
		harmful_enemies = 0;
		budget = 0;

		is_started = true;
		SetPaused(false);
		is_practice = false;
		restart_text.text = "Restart";
		resume_button.interactable = true;
		core = Instantiate(crawler, transform.position, transform.rotation);
		core_segment = core.GetComponentInChildren<CoreSegment>();

		round_count_text.text = "" + current_wave;
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
		Time.timeScale = paused ? 0 : 1;
		hud.SetActive(!paused);
		menu.SetActive(paused);
	}

	public void FixedUpdate() {
		if (spawn_amount > 0) {
			int index = UnityEngine.Random.Range(0, Math.Min(enemy_types.Length, spawn_amount));
			SpawnEnemy(enemy_types[index]);
			spawn_amount -= index + 1;
		}else if (num_bosses > 0) {
			SpawnEnemy(mothership);
			num_bosses -= 1;
		}

		enemy_count_text.text = "" + enemies.Count;

		bool round_continues = harmful_enemies > 0;

		enemy_count_text.gameObject.SetActive(round_continues);
		next_wave_button.gameObject.SetActive(!round_continues);
		next_wave_button.interactable = !round_continues;
	}

	public void NextWave() {
		if (is_practice) {
			return;
		}

		current_wave += 1;
		spawn_amount += current_wave * 2 + 5 + budget * 5;
		budget = 0;

		if ((current_wave) % bosses_at == 0) {
			num_bosses += (current_wave) / bosses_at;
		}
		else if (current_wave >= 25) {
			num_bosses += (int) (UnityEngine.Random.value * current_wave / bosses_at);
		}

		round_count_text.text = "" + current_wave;
		if (highscore < current_wave) {
			highscore = current_wave;
			highscore_text.text = "" + highscore;
		}
	}

	private void SpawnEnemy(GameObject enemy) {
		Instantiate(enemy, transform.position + Vector3.up * 40, transform.rotation);
	}

	public void AddEnemy(Enemy enemy) {
		enemies.Add(enemy);
		if (enemy.is_harmful) {
			harmful_enemies += 1;
		}
	}

	public void RemoveEnemy(Enemy enemy) {
		enemies.Remove(enemy);
		if (enemy.is_harmful) {
			harmful_enemies -= 1;
		}
	}

	public void AddSegment(ConstructionSegment segment) {
		segments.Add(segment);
	}

	public void RemoveSegment(ConstructionSegment segment) {
		segments.Remove(segment);
	}

	public void AddShot(Shot shot) {
		shots.Add(shot);
	}

	public void AddBomb(Bomb bomb) {
		bombs.Add(bomb);
	}

	public void RemoveShot(Shot shot) {
		shots.Remove(shot);
	}

	public void SetSegment(GameObject to_place) {
		segment_placer.SetSegment(to_place);
	}

	public int GetZollars() {
		return zollars;
	}

	private void ZollarsChanged(int change) {
		if (change == 0) {
			return;
		}

		zollars += change;
		zollar_text.text = "" + zollars;

		GameObject new_balance_change = Instantiate(balance_change_indicator, zollar_text.transform);

		new_balance_change.transform.position -= ((RectTransform) zollar_text.transform).sizeDelta.y * Vector3.up * 4;

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

	public void ResetZollars() {
		ZollarsChanged(-zollars);
	}

	public void AddZollars(int amount) {
		ZollarsChanged(amount);
	}

	public void RemoveZollars(int amount) {
		ZollarsChanged(-amount);
	}

	public void AddBoss() {
		boss_count += 1;
	}

	public void RemoveBoss() {
		boss_count -= 1;
	}

	public void AddBudget(int amount) {
		budget += amount;
	}
}
