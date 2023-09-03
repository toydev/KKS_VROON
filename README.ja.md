[English](README.md)

# KKS_VROON - Simple VR plugin for Koikatsu Sunshine
**横になって片手で遊べるメインゲーム/CharaStudio 両対応の VR MOD です。**

KKS には様々な VR の選択肢があります。本 MOD の主要な特徴は以下の通りです。

- いつでも正面をリセットできる。座って／立って／横になって、あなたの好きな姿勢で遊べる。
- 片手の VR コントローラーだけで全てのマウス操作ができる。もう片方の手を自由にできる。

----

# サポートしている HMD
- Meta Quest 2

サポート追加のプルリクエスト歓迎です。

----

# 前提
- コイカツ！サンシャイン
- 最新バージョンの BepInEx 5.X と KKAPI/ModdingAPI
- SteamVR
- Meta Quest 2

- 他の VR 化 mod が入っていないこと

----

# 遊び方
ゲームに [KKS_VROON](https://github.com/toydev/KKS_VROON/releases) をインストールし、HMD と SteamVR を接続してゲームを開始してください。

開始時に SteamVR のプロセスを検出して有効になります。

----

# 操作
## マウスと VR コントローラー
左右の VR コントローラーの操作は両方とも同じです。

|マウス操作|VR コントローラー操作|
|----|----|
|左クリック|トリガー押下|
|右クリック|グリップ押下|
|中央クリック|X / A 押下|
|メニューをポインターで選択|メニューをレーザーポインターで選択|
|部位をポインターで選択|部位をレーザーポインターで選択|
|左 / 右クリック + マウス移動|トリガー / グリップ押下 + ジョイスティック（※メインゲームでの歩行時、縦操作は抑制します）|
|マウスホイール操作|ジョイスティックの上下|

## VR コントローラーのみ
|VR コントローラー操作|アクション|
|----|----|
|Y / B 押下|VR カメラの正面をリセット|

----

## 設定オプション
このプラグインは以下の設定オプションを提供します。

|設定名|型|デフォルト値|説明|
|----|----|----|----|
|GameWindowTopMost|Boolean|`true`|ゲームウィンドウを常に最前面に表示する。|
|EnableMirror|Boolean|`false`|VR内でミラーを有効/無効にする。|
|MainGameJoystickViewSpeed|Float|`30.0f`|ジョイスティックによる視点移動の速度を調整するためのメインゲーム用のスケーリング係数。|
|CharaStudioJoystickViewSpeed|Float|`50.0f`|ジョイスティックによる視点移動の速度を調整するための CharaStudio 用のスケーリング係数。|
|MouseWheelScalingFactor|int|`120`|マウスホイールのスケーリング係数。|

これらの設定は BepInEx の設定マネージャーで変更できます。

----

# 開発者向け
## Issues / Pull requests
英語か日本語でお願いします。

- バグ報告
- VR コントローラーサポート追加
- その他
  - 実装してみたいと思う機能があるなら、このプロジェクトについての知識を提供します。

CharaStudio を普段使っていないので、CharaStudio に関連する内容は詳しく書いてください。

## 資料
- [このプロジェクトのセットアップ方法](/docs/project/HOW_TO_CREATE_STEAMVR_PROJECT.md)
- [基本的な実装コンセプト](/docs/project/BASIC_IMPLEMENTATION_CONCEPTS.md)
- [VR コントローラーについて](/docs/project/ABOUT_VR_CONTROLLER.md)
- [SteamVR プロジェクトを０から作る方法](/docs/project/HOW_TO_CREATE_STEAMVR_PROJECT.md)
