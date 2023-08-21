[English](README.md)

# KKS_VROON - Simple VR plugin for Koikatsu Sunshine

**横になって片手で遊べるメインゲーム/CharaStudio 両対応の VR MOD です。**

KKS には様々な VR の選択肢があります。本 MOD の主要な特徴は以下の通りです。

- いつでも正面をリセットできる。座って／立って／横になって、あなたの好きな姿勢で遊べる。
- 片手のハンドコントローラーだけで全てのマウス操作ができる。もう片方の手を自由にできる。

----

# サポートしているコントローラー

- Meta Quest 2

開発者の所持状況を反映しています。サポート追加のプルリクエスト歓迎です。

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
## マウスとハンドコントローラー
左右のハンドコントローラーの操作は両方とも同じです。

|マウス操作|ハンドコントローラー操作|
|----|----|
|左クリック|トリガー押下|
|右クリック|グリップ押下|
|中央クリック|X / A 押下|
|メニューをポインターで選択|メニューをレーザーポインターで選択|
|部位をポインターで選択|部位をレーザーポインターで選択|
|メインゲーム移動時の横視点移動|ジョイスティック|
|メインゲーム移動時の縦視点移動|HMD で見上げてください|

## ハンドコントローラーのみ
|ハンドコントローラー操作|アクション|
|----|----|
|Y / B 押下|VR カメラの正面をリセット|

----

# 開発者向け

- [このプロジェクトのセットアップ方法](/docs/project/HOW_TO_CREATE_STEAMVR_PROJECT.md)
- [基本的な実装コンセプト](/docs/project/BASIC_IMPLEMENTATION_CONCEPTS.md)
- [SteamVR プロジェクトを０から作る方法](/docs/project/HOW_TO_CREATE_STEAMVR_PROJECT.md)
