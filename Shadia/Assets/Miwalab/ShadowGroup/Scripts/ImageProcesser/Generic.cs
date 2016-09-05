using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public enum ImageProcesserType
    {
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
        /// 一定方向に流れるパーティクル
        /// </summary>
        FlowParticlesShadow,

        /// <summary>
        /// 個数を数えるためのもの．消したら処刑
        /// </summary>
        Count,
    }

}
