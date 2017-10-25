using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public enum ImageProcesserType
    {
        /// <summary>
        /// 変更
        /// </summary>
        Selector,

        /// <summary>
        /// 通常影　自影
        /// </summary>
        Normal,
        /// <summary>
        /// きれい　自影
        /// </summary>
        VividNormal,
        /// <summary>
        /// 黒
        /// </summary>
        Black,
        /// <summary>
        /// 白
        /// </summary>
        White,
        /// <summary>
        /// 二重残像
        /// </summary>
        DoubleAfterImage,
        /// <summary>
        /// ハリネズミ影
        /// </summary>
        Spike,
        
        /// <summary>
        /// ポリゴン
        /// </summary>
        Polygon,
        
        /// <summary>
        /// 時間遅れ影
        /// </summary>
        TimeDelay,
        /// <summary>
        /// 田村スケルトン
        /// </summary>
        TamuraSkeleton,
        
        /// <summary>
        /// パーティクル影
        /// </summary>
        Particle,
        /// <summary>
        /// CA
        /// </summary>
        CellAutomaton,
        /// <summary>
        /// 二次元パーティクル
        /// </summary>
        Particle2D,
        /// <summary>
        /// 二次元パーティクル　際に集まってくる
        /// </summary>
        ParticleVector,
        /// <summary>
        /// 個々の操作を加えるパーティクル
        /// </summary>
        EachMoveParticle,

        /// <summary>
        /// もち（B5河野）
        /// </summary>
        Attraction,

        /// <summary>
        /// 手のひらくっつくやつ（B5河野）
        /// </summary>
        HandsTo,

        /// <summary>
        /// 手動かすやつ（B5河野）
        /// </summary>
        HandElbow,

        /// <summary>
        /// 線が出るやつ（B5河野）
        /// </summary>
        Electrical,

        /// <summary>
        /// 輪郭線のやつ（B5河野）
        /// </summary>
        Canny,

        /// <summary>
        /// ボーンで影を動かす（B5河野）
        /// </summary>
        MoveShadow,

        /// <summary>
        /// 二重に影を提示するやつ（B5河野）
        /// </summary>
        Twins,

        /// <summary>
        /// 先行して動く影（B5河野）
        /// </summary>
        Ahead,

        /// <summary>
        /// いろんな色が出る影（B5河野）
        /// </summary>
        Colorful,

        /// <summary>
        /// 最小二乗法二次曲線近似先行 輪郭重心（B5河野）
        /// </summary>
        LeastSquare,

        /// <summary>
        /// 最小二乗法二次曲線近似先行　骨格点（B5河野）
        /// </summary>
        LSAhead,

        /// <summary>
        /// 色がまざりあう影（B5河野）
        /// </summary>
        MixColor,

        /// <summary>
        /// ポイントクラウドの基本画像処理（B5河野）
        /// </summary>
        PtsImgProcesser,

        /// <summary>
        /// 明るさ測定用メディア（B5河野）
        /// </summary>
        BrightCheck,

        /// <summary>
        /// Normal影の色を変えるバージョン（B5河野）
        /// </summary>
        ChangeColor,

        /// <summary>
        /// 輪郭ごとに色の違う影（B5河野）
        /// </summary>
        PersonalColor,

        /// <summary>
        /// 秒数をパラメーターとした時間遅れ影（B5河野）
        /// </summary>
        SecondDelay,

        /// <summary>
        /// 自分を追いかけてくる影（B5河野）
        /// </summary>
        BodyChase,

        /// <summary>
        /// 一定方向に流れるパーティクル
        /// </summary>
        FlowParticlesShadow,

        /// <summary>
        /// 描画する影メディア
        /// </summary>
        PainterShadow,

        /// <summary>
        /// 3DParticle
        /// </summary>
        Particle3D,

        /// <summary>
        /// 田中練習
        /// </summary>
        TanakaTest,

        /// <summary>
        /// 個数を数えるためのもの．消したら処刑
        /// </summary>
        Count,

    }

}
