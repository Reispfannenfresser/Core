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
	Text kills_text = null;
	[SerializeField]
	Text next_round_text = null;
	[SerializeField]
	Text round_count_text = null;
	[SerializeField]
	Text restart_text = null;
	[SerializeField]
	Text highscore_text = null;
	[SerializeField]
	Button resume_button = null;

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
	float next_wave_in = 20;
	int zollars = 150;
	int kills = 0;
	public bool is_paused = true;
	public bool is_started = true;
	public bool is_practice = false;

	public int boss_count = 0;

	[SerializeField]
	private Color ui_color = Color.white;
	[SerializeField]
	private Color ui_selected_color = Color.white;

	int score = 0;
	int highscore = 0;

	int spawn_amount = 0;
	int num_bosses = 0;

	public static GameController instance = null;

	public HashSet<Enemy> enemies = new HashSet<Enemy>();
	public HashSet<ConstructionSegment> segments = new HashSet<ConstructionSegment>();
	public HashSet<Shot> shots = new HashSet<Shot>();

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
				segment.Delete();
			}
		}

		foreach(Shot shot in shots) {
			if (shot != null) {
				Destroy(shot.gameObject);
			}
		}

		current_wave = 0;
		next_wave_in = 25;
		zollars = 250;
		kills = 0;
		boss_count = 0;
		num_bosses = 0;
		spawn_amount = 0;

		is_started = true;
		SetPaused(false);
		is_practice = false;
		restart_text.text = "Restart";
		resume_button.interactable = true;
		core = Instantiate(crawler, transform.position, transform.rotation);
		core_segment = core.GetComponentInChildren<CoreSegment>();
	}

	public void StartPractice() {
		StartGame();
		restart_text.text = "Start";
		is_practice = true;
		next_wave_in = 5;
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
		next_wave_in -= Time.deltaTime;
		if (next_wave_in < 0) {
			next_wave_in = 0;
		}

		if (boss_count > 0 && next_wave_in < 5) {
			next_wave_in = 5;
		}

		if (current_wave > 5 && enemies.Count == 0 && next_wave_in > 5) {
			next_wave_in = 5;
		}

		if (current_wave > highscore && enemies.Count == 0) {
			score = current_wave;
			if (score > highscore) {
				highscore = score;

			}
		}

		if (next_wave_in <= 0) {
			NextWave();
		}

		if (spawn_amount > 0) {
			int index = UnityEngine.Random.Range(0, Math.Min(enemy_types.Length, spawn_amount));
			SpawnEnemy(enemy_types[index]);
			spawn_amount -= index + 1;
		}

		if (num_bosses > 0) {
			SpawnEnemy(mothership);
			num_bosses -= 1;
		}

		next_round_text.text = "" + Mathf.Ceil(next_wave_in);
		round_count_text.text = "" + Mathf.Max(score - 1, 0);
		highscore_text.text = "" + Mathf.Max(highscore - 1, 0);
		zollar_text.text = "" + zollars;
		kills_text.text = "" + kills;
	}

	public void NextWave() {
		if (is_practice) {
			AddZollars(250);
			next_wave_in = 5;
			return;
		}

		current_wave += 1;

		spawn_amount += current_wave * 2;

		next_wave_in = current_wave + 5;

		if ((current_wave - 1) % bosses_at == 0) {
			num_bosses += (current_wave - 1) / bosses_at;
		}

		if (current_wave > 25) {
			num_bosses += (int) (UnityEngine.Random.value * current_wave / 10);
		}
	}

	private void SpawnEnemy(GameObject enemy) {
		Instantiate(enemy, transform.position + Vector3.up * 40, transform.rotation);
	}

	public void AddEnemy(Enemy enemy) {
		enemies.Add(enemy);
	}

	public void RemoveEnemy(Enemy enemy) {
		enemies.Remove(enemy);
		kills++;
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

	public void RemoveShot(Shot shot) {
		shots.Remove(shot);
	}

	public void SetSegment(GameObject to_place) {
		segment_placer.SetSegment(to_place);
	}

	public int GetZollars() {
		return zollars;
	}

	public void AddZollars(int amount) {
		zollars += amount;
	}

	public void RemoveZollars(int amount) {
		zollars -= amount;
	}

	public void AddBoss() {
		boss_count += 1;
	}

	public void RemoveBoss() {
		boss_count -= 1;
	}
}
