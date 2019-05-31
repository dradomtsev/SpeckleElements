﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeckleElementsGSA
{
  public enum GSATargetLayer
  {
    Design,
    Analysis
  }

  /// <summary>
  /// Number of nodes in each GSA element definition
  /// </summary>
  public enum ElementNumNodes
  {
    BAR = 2,
    BEAM = 2,
    BEAM3 = 3,
    BRICK20 = 20,
    BRICK8 = 8,
    CABLE = 2,
    DAMPER = 2,
    GRD_DAMPER = 1,
    GRD_SPRING = 1,
    LINK = 2,
    MASS = 1,
    QUAD4 = 4,
    QUAD8 = 8,
    ROD = 2,
    SPACER = 2,
    SRING = 2,
    STRUT = 2,
    TETRA10 = 10,
    TETRA4 = 4,
    TIE = 2,
    TRI3 = 3,
    TRI6 = 6,
    WEDGE15 = 15,
    WEDGE6 = 6
  };

  /// <summary>
  /// GSA category section ID
  /// </summary>
  public enum GSACAtSectionType
  {
    I = 1,
    CastellatedI = 2,
    Channel = 3,
    T = 4,
    Angles = 5,
    DoubleAngles = 6,
    CircularHollow = 7,
    Circular = 8,
    RectangularHollow = 9,
    Rectangular = 10,
    Oval = 1033,
    TwoChannelsLaces = 1034
  }

  /// <summary>
  /// GSA 2D element layer
  /// </summary>
  public enum GSA2DElementLayer
  {
    Bottom = 0x1,
    Middle = 0x2,
    Top = 0x4,
  }

  public enum GSAResultMode
  {
    NodeReactions,
    NodeDisplacements,
    Element1DForces,
    Element2DDisplacements,
    Element2DForces,
    Element2DStresses,
  }

  public enum GSAEntity
  {
    NODE = 1,
    ELEMENT = 2,
    MEMBER = 3,
    LINE = 6,
    AREA = 7,
    REGION = 8
  }
}