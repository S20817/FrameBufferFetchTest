# 問題説明

BlurFilterなど近隣ピクセルをロードする必要がある処理の直後に、renderGraph.AddCopyPass(FrameBufferFetch使用)でコピーしようとするとこのように問題が出ます。
<img width="957" height="536" alt="image" src="https://github.com/user-attachments/assets/abc40882-3e01-4066-9e44-0a49d5626961" />

Blur処理PassとCopy処理Passがマージされたことから見ると、Blur処理もTileBasedRenderingになってるみたいで、それが問題だなと思います。

そのマージは強制的で、近隣ピクセルとやり取りする処理の直後にAddCopyPass(FrameBufferFetch)使うと全部こうなるかと思います。

近隣ピクセルとやり取りする処理の直後にAddCopyPass(FrameBufferFetch)を使ってはいけない？

## Unityに問い合わせした不具合の原因

UnityのAddCopyPass(FrameBufferFetchを使う自作パスも同じ)を使うと、そのSourceが直前パスのOutputであり、かつ解像度やMSAAなどの属性が同じであれば、直前のPassの行為に問わずにマージされることからだそうです。

当ケースの場合、直前のPassが近隣ピクセルサンプリングしているため、マージ後、タイルメモリ上で近隣ピクセルサンプリングすることになり、タイル外のメモリにアクセスしたのでその描画不具合が発生します

<img width="818" height="443" alt="image" src="https://github.com/user-attachments/assets/53850e9a-c41e-4f40-a804-07c739ea2097" />

## Issueが立てられた
https://issuetracker.unity3d.com/issues/using-addcopypass-causes-an-incorrect-merging-of-passes

# 再現プロジェクト説明

windowsでは再現しない、Macで開く必要があります。(TileBasedRenderingGPU搭載の端末が必要)

Unityバージョン：6000.1.13f1 (それ以下のももちろん発生する)
シーン: SampleScene.unity

関連ファイル
- スクリプト: Assets/Scriptsフォルダ以下
- シェーダー: Assets/Materialsフォルダ以下

再現設定：すでに再現する設定しているので開けば再現するはず

MainCameraにアタッチされているCustomBlurRenderで設定を変えることができます
<img width="592" height="209" alt="image" src="https://github.com/user-attachments/assets/08841a16-002b-47eb-80f4-95c245801b0a" />

- FilterTypeをNoFilterにすれば問題が解消する
  - 近隣ピクセルサンプリングしないため、タイルメモリとのやりとりに問題なし
- Use Unsafe Pass For Blurをチェックすると問題が解消する
  - BlurをUnsafePassにすることで、後続のCopyPassをマージしなくなるため、タイルメモリ上で近隣ピクセルサンプリングすることはなくなる
- Use Frame Buffer Fetch For Copy Backのチェックを外すと問題が解消する
  - Copy処理にタイルメモリ上で完結しないようになったので、もちろん問題がなくなる
 



