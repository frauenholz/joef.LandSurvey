module joef.landEngineering.LandArea

#light

open System
open System.Runtime.Serialization

// zero cartesian degrees is equal to true north
//          W, 90
//  SW = II   |    NW = I
//            |
// S, 180 --------- N, 0, 360
//            |
//  SE = III  |    NE = IV
//          E, 270

//  N 90° 0' 0" W == S 90° 0' 0" W
//  N 90° 0' 0" E == S 90° 0' 0" E

[<DataContract>]
type NS = 
  | [<EnumMember>] N = 0
  | [<EnumMember>] S = 180
  | [<EnumMember>] none = 0  // NS is meaningless when DMS is 90 || 270



[<DataContract>]
type EW = 
  | [<EnumMember>] W = 90
  | [<EnumMember>] E = 270
  | [<EnumMember>] none = 0  // EW is meaningless when DMS is 0 || 180




let GetCartesianDegrees (ns:NS, 
                         degrees:uint16, 
                         minutes:uint16, 
                         seconds:float, 
                         ew:EW) =
  let dmsToFloat = float degrees + float minutes/60.0 + float seconds/360.0
  if ns = NS.N then
    if ew = EW.W then    
      0.0 + dmsToFloat    // N - W
    else                    
      360.0 - dmsToFloat  // N - E
  else                        
    if ew = EW.W then        
      180.0 - dmsToFloat  // S - W
    else                    
      180.0 + dmsToFloat  // S - E




type CompassBearing =
    val ns:NS
    val degrees:uint16
    val minutes:uint16
    val seconds:float
    val ew:EW

    new ( ns:NS, degrees:uint16, minutes:uint16, seconds:float, ew:EW ) =
        {   
            ns = ns;
            degrees = degrees;
            minutes = minutes;
            seconds = seconds;
            ew = ew;
        }
        
    override x.ToString() = 
        let result = x.ns.ToString() + " " + x.degrees.ToString() + "° " + x.minutes.ToString()  + "' " + x.seconds.ToString()  + "\" " + x.ew.ToString()
        result




let GetCompassBearing (cartesianDegrees:float) : CompassBearing = 
  let degWholeF    = floor cartesianDegrees;
  let degFraction  = ( cartesianDegrees - degWholeF ) * 60.0
  let minWholeF    = floor degFraction
  let minFraction  = degFraction - minWholeF

  let minWhole     = uint16 minWholeF
  let degWhole     = uint16 degWholeF
  let secWhole     = float (round ( minFraction * 60.0) )

  let result = 
    if cartesianDegrees = 0.0 then
      CompassBearing(NS.N, 0us, 0us, 0.0, EW.none)
    else
    if cartesianDegrees > 0.0 && 
       cartesianDegrees < 90.0 then
      CompassBearing(NS.N, degWhole, minWhole, secWhole, EW.W)
    else
    if cartesianDegrees = 90.0 then
      CompassBearing(NS.N, 90us, 0us, 0.0, EW.W)
    else
    if cartesianDegrees > 90.0 && 
        cartesianDegrees < 180.0 then
      CompassBearing(NS.S, 180us - degWhole, minWhole, secWhole, EW.W)
    else
    if cartesianDegrees = 180.0 then
      CompassBearing(NS.S, 0us, 0us, 0.0, EW.none)
    else
    if cartesianDegrees > 180.0 && 
        cartesianDegrees < 270.0 then
      CompassBearing(NS.S, degWhole - 180us, minWhole, secWhole, EW.E)
    else
    if cartesianDegrees = 270.0 then
      CompassBearing(NS.S, 0us, 0us, 0.0, EW.E)
    else
    if cartesianDegrees > 270.0 && 
       cartesianDegrees < 360.0 then
      CompassBearing(NS.N, 360us - degWhole, minWhole, secWhole, EW.E)
    else
      CompassBearing(NS.N, 0us, 0us, 0.0, EW.none)
  result




type Quadrant =     // compass bearing to cartesian quadrant
  | none = 0    // 0, 90, 180, 270
  | NW = 1      // 0   < 90
  | SW = 2      // 90  < 180
  | SE = 3      // 180 < 270
  | NE = 4      // 270 < 360



let GetQuadrant (ns, ew) =
  if ns = NS.N && ew = EW.W then
    Quadrant.NW
  else
  if ns = NS.S && ew = EW.W then
    Quadrant.SW
  else
  if ns = NS.S && ew = EW.E then
    Quadrant.SE
  else
  if ns = NS.N && ew = EW.E then
    Quadrant.NE
  else
    Quadrant.none




type Direction =    // stores direction info as a "Compass Bearing" and a "Cartesian Angle"
  val CartesianDegrees:float
  val CompassBearing:CompassBearing

  member x.Quadrant:Quadrant = 
    let ns = x.CompassBearing.ns
    let ew = x.CompassBearing.ew
    GetQuadrant(ns, ew)

  member x.QuadrantNum = int x.Quadrant

  new (cartesianDegrees:float) = 
    { CartesianDegrees = cartesianDegrees;
      CompassBearing = GetCompassBearing(cartesianDegrees) }

  new (ns:NS, degrees:uint16, minutes:uint16, seconds:float, ew:EW) = 
    { CartesianDegrees = GetCartesianDegrees(ns, degrees, minutes, seconds, ew);
      CompassBearing = CompassBearing(ns, degrees, minutes, seconds, ew) }

  override x.ToString() = 
    let bearing = x.CompassBearing.ToString()
    let quadrant = 
      match x.QuadrantNum with
        | 1 -> "I"
        | 2 -> "II"
        | 3 -> "III"
        | 4 -> "IV"
        | _ -> ""
    bearing + " • " + x.CartesianDegrees.ToString() + "° • " + quadrant




type Vector =
  val Direction:Direction
  val Length:float

  new (a, b, c, d, e, length:float) =
    { Direction = new Direction ( a, b, c, d, e );
      Length = length }

//  new ((a, b, c, d, e):NS * uint16 * uint16 * uint16 * EW , length:float) =
//    { Direction = new Direction ( a, b, c, d, e );
//      Length = length }

  new (cartesianDegrees:float, length:float) =
    { Direction = new Direction(cartesianDegrees);
      Length = length }

//  new (direction:Direction, length:float) =
//    { Direction = direction;
//      Length = length }

  override x.ToString() = 
    x.Direction.ToString()  + " | " + x.Length.ToString()

    




    // Algoritm for determinining rotation direction
//  for any 1st vector, there is a vector 180° out of phase with the 1st vector, that bisects the cartesian quadrants
//  if the 1st vector is in quadrant I or II, and if the 2nd vector is less than the 1st vector, or the 2nd vector is less than 360°
//  and greater than the 180° out of phase vector, then the rotation of the 2nd vector with respect to the 1st vector is CW and 
//  if the 1st vector is in quadrant III or IV, and if the 2nd vector is greater than the 1st vector, or the 2nd vector is greater than 0°
//  less than the 180° out of phase vector, then the rotation of the 2nd vector with respect to the 1st vector is CCW

type ClockRotation =     // cartesian rotation from one vector to the next
  | CW = -1
  | CCW = 1
  | straigtAhead = 0
  | reverse = 180




type CartesianTurn (cartesianDegrees1:float, cartesianDegrees2:float) =

  // all rotations (angular differences) are positive going CCW

  let vectorOneInQuadrantOneTwo    = 180.0 > cartesianDegrees1 && cartesianDegrees1 > 0.0
  let vectorOneInQuadrantThreeFour = 180.0 < cartesianDegrees1 && cartesianDegrees1 < 360.0
  let vectorOne180DegreesOutOfPhase =
    if vectorOneInQuadrantOneTwo then
      cartesianDegrees1 + 180.0
    else
    if vectorOneInQuadrantThreeFour then
      cartesianDegrees1 - 180.0
    else
    if cartesianDegrees1 = 180.0 then
      0.0
    else
      180.0

  let vector2LessThanVector1 = cartesianDegrees2 < cartesianDegrees1
  let vector2LessThan360AndGreaterThanOutOfPhase  = cartesianDegrees2 < 360.0 && cartesianDegrees2 > vectorOne180DegreesOutOfPhase
  let vector2GreaterThanVector1 = cartesianDegrees2 > cartesianDegrees1
  let vector2GreaterThan0AndLessThanOutOfPhase = cartesianDegrees2 > 0.0 && cartesianDegrees2 < vectorOne180DegreesOutOfPhase

  member x.RotationClock =
    if vectorOneInQuadrantOneTwo then
      if vector2LessThanVector1 || vector2LessThan360AndGreaterThanOutOfPhase then
        ClockRotation.CW
      else
        ClockRotation.CCW
    else
    if vectorOneInQuadrantThreeFour then
      if vector2GreaterThanVector1 || vector2GreaterThan0AndLessThanOutOfPhase then
        ClockRotation.CCW
      else
        ClockRotation.CW
    else
    if cartesianDegrees1 = cartesianDegrees2 then 
      ClockRotation.straigtAhead
    else
      ClockRotation.reverse

  member x.RotationFactor = int x.RotationClock

  // use exterior angle to calculate angle C
  member t.C =
    if vectorOneInQuadrantOneTwo && vector2LessThanVector1 then
      180.0 - (cartesianDegrees1 - cartesianDegrees2)
    else
    if vectorOneInQuadrantOneTwo && vector2LessThan360AndGreaterThanOutOfPhase then
      cartesianDegrees2 - vectorOne180DegreesOutOfPhase
    else
    if vectorOneInQuadrantOneTwo then
      180.0 - (cartesianDegrees2 - cartesianDegrees1)
    
    else
    if vectorOneInQuadrantThreeFour && vector2GreaterThanVector1 then
      180.0 - (cartesianDegrees2 - cartesianDegrees1)
    else
    if vectorOneInQuadrantThreeFour && vector2GreaterThan0AndLessThanOutOfPhase then
      vectorOne180DegreesOutOfPhase - cartesianDegrees2
    else
    if vectorOneInQuadrantThreeFour then
      180.0 - (cartesianDegrees1 - cartesianDegrees2)

    else
    if cartesianDegrees1 = 180.0 || cartesianDegrees1 = 0.0 then
      abs(180.0 - cartesianDegrees2)
    else
      -1000.0




//    |\
//   b| \c
//    |  \
//    ----*
//      a

type Triangle (vector1:Vector, vector2:Vector) =
  let DegreesToRadians degrees:float = degrees * Math.PI / 180.0
  let RadiansToDegrees radians:float = radians * 180.0 / Math.PI

  member t.a = vector1.Length
  member t.b = vector2.Length

  member t.cartesianDegrees1 = vector1.Direction.CartesianDegrees
  member t.cartesianDegrees2 = vector2.Direction.CartesianDegrees
  member t.Turn = new CartesianTurn(t.cartesianDegrees1, t.cartesianDegrees2)

  member t.RotationClock = t.Turn.RotationClock
  member t.C = t.Turn.C

  member t.Area = t.a * t.b * sin(DegreesToRadians(t.C)) / 2.0

  member t.c = sqrt(t.a ** 2.0 + t.b ** 2.0 - 2.0 * t.a * t.b * cos(DegreesToRadians t.C))
  member t.A = RadiansToDegrees(acos( (t.b ** 2.0 + t.c ** 2.0 - t.a ** 2.0)/ (2.0 * t.b * t.c) ) )
  member t.B = RadiansToDegrees(acos( (t.a ** 2.0 + t.c ** 2.0 - t.b ** 2.0)/ (2.0 * t.a * t.c) ) )
  // note: using the Law of Sines to calculate angles may cause ambiguos results

  member t.cartesianDegrees3 = 
    let angle = t.cartesianDegrees1 + float t.Turn.RotationFactor * t.B
    if ( angle >= 360.0 ) then
      angle - 360.0
    else
      angle
  
  member t.sumAngles = t.A + t.B + t.C