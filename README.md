# 問題説明

BlurFilterなど近隣ピクセルをロードする必要がある処理の直後に、renderGraph.AddCopyPass(FrameBufferFetch使用)でコピーしようとするとこのように問題が出ます。
<img width="957" height="536" alt="image" src="https://github.com/user-attachments/assets/abc40882-3e01-4066-9e44-0a49d5626961" />

Blur処理PassとCopy処理Passがマージされたことから見ると、Blur処理もTileBasedRenderingになってるみたいで、それが問題だなと思います。

そのマージは強制的で、近隣ピクセルとやり取りする処理の直後にAddCopyPass(FrameBufferFetch)使うと全部こうなるかと思います。

近隣ピクセルとやり取りする処理の直後にAddCopyPass(FrameBufferFetch)を使ってはいけない？
それがUnityの不具合か仕様か、Unityに問い合わせ中
<img width="818" height="443" alt="image" src="https://github.com/user-attachments/assets/53850e9a-c41e-4f40-a804-07c739ea2097" />

# 再現プロジェクト説明

windowsでは再現しない、Macで開く必要があります。(TileBasedRenderingGPU搭載の端末が必要)

シーン: SampleScene.unity

関連ファイル
- スクリプト: Assets/Scriptsフォルダ以下
- シェーダー: Assets/Materialsフォルダ以下

再現設定：すでに再現する設定しているので開けば再現するはず

MainCameraにアタッチされているCustomBlurRenderで設定を変えることができます
<img width="478" height="157" alt="image" src="https://github.com/user-attachments/assets/5bcf934a-1a5c-4cef-af55-df51f0d0b36e" />
