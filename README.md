# 翔け小泉信じろー！エナジー＆必殺技システム

無限ランナー「翔け小泉信じろー！」に、ジャンプでエナジーを溜めて全障害物を一掃する必殺技を追加しました。Unity 6.2 (Universal Render Pipeline 2D) で動作するよう設計されています。

---

## 追加機能の概要

- **エナジーゲージ**: ジャンプ（マウス左クリック）ごとに 10% 蓄積。最大 100% でストック完了。
- **必殺技トリガー**: エナジー 100% 時に右クリック（または `Space`）で発動。画面内の「Obstacle」タグ付きオブジェクトを全滅。
- **フィードバック演出**: パーティクル、画面フラッシュ、サウンドのフックを用意。Inspector 上で簡単に差し替え可能。
- **スコアシステム連携**: 既存の「国民の評価」スコアは継続的に加算。ゲームオーバー時はハイスコアを保存。

---

## シーン設定手順

1. **スクリプトの配置**
   - `PlayerController` をプレイヤーキャラクターにアタッチ。
   - `GameManager` を空の管理オブジェクトにアタッチ（`AudioSource` が自動で追加されます）。

2. **UI の準備**
   - Canvas 内に TextMeshPro UI を 2 つ用意。
     - `scoreText`: 既存のスコア表示に割り当て。
     - `energyText`: 新規「エナジー」表示（右上など）に割り当て。
   - `GameManager` の Inspector で `Energy Text` に TextMeshProUGUI をドラッグ。

3. **画面フラッシュ（任意）**
   - Canvas 内に `Image` などで全画面を覆う UI を作成し `CanvasGroup` を追加。
   - `Alpha` を 0 に設定し、`Interactable` と `Blocks Raycasts` をオフ。
   - `GameManager` の `Screen Flash Canvas` にその `CanvasGroup` を割り当て。

4. **パーティクル＆サウンド（任意）**
   - 必殺技演出用の `ParticleSystem` プレハブを用意し `Ultimate Effect Prefab` に設定。
   - 爆発サウンドなどを `AudioClip` として `Ultimate Sfx` に設定。
   - プレイヤーのジャンプ音があれば `PlayerController` の `Jump Audio Source` に割り当て。

5. **地面判定**
   - プレイヤーの足元に空オブジェクトを作成し `Ground Check` に設定。
   - `Ground Check Radius` と `Ground Layers` を適切に設定してジャンプ制御を調整。

6. **タグ管理**
   - 障害物: `Obstacle` タグを付与（必殺技で破壊される対象）。
   - アイテム: `Item` タグを付与（取得時にスコア加算）。

7. **リスタートボタン**
   - UI にボタンを用意し `GameManager` の `Restart Button` に割り当て。
   - `Button` の OnClick() に `GameManager.Restart()` を紐付け。

---

## 操作方法

| アクション | 入力 | 備考 |
| --- | --- | --- |
| ジャンプ | マウス左クリック | エナジー +10% |
| 必殺技 | マウス右クリック / Space | エナジー 100% 時のみ発動 |
| リスタート | UI ボタン | ゲームオーバー時のみ表示 |

Inspector から `Ultimate Key` や `Ultimate Mouse Button` を変更すれば、キーボード専用操作への切り替えも可能です。

---

## スクリプト構成

- `Assets/Scripts/PlayerController.cs`
  - 前進移動、ジャンプ、エナジー蓄積、必殺技トリガーを制御。
- `Assets/Scripts/GameManager.cs`
  - スコア更新、エナジー UI、必殺技演出、ゲームオーバー処理を担当。

---

## デバッグのヒント

- 実行中に `GameManager` を選択すると、Inspector で現在のスコアやエナジー値（0〜100）を確認できます。
- 障害物が消えない場合はタグ設定を再確認してください。
- パーティクルやサウンドが再生されない場合は、Inspector の参照が欠けていないかチェックしてください。

---

## 既知の制約

- Unity エディタ外（CLI）ではテストを実行していません。挙動確認は Unity エディタ内で行ってください。
- `FindFirstObjectByType<GameManager>()` を使用しているため、シーン内に `GameManager` が 1 つ配置されている必要があります。

---

## 次のステップ（任意）

- 必殺技発動時に追加のスコアボーナスを与えるなど、報酬設計を強化。
- エナジーの蓄積量をアイテムやコンボで変化させ、戦略性を拡張。
- Screen shake、ポストエフェクトなどの演出を加えてさらなる爽快感を演出。
