﻿using System;
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
        /// 一定方向に流れるパーティクル
        /// </summary>
        FlowParticlesShadow,

        /// <summary>
        /// 個数を数えるためのもの．消したら処刑
        /// </summary>
        Count,
    }

}
