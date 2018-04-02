namespace GottaMusicFast
open System.Numerics

type MusicInformation = {Treble:decimal; Tenor:decimal; Bass:decimal; Seventh:decimal; Volume:decimal; }

type AccelerometerInformation = {Speed:decimal; CurrentRotation:Vector3; Previous:AccelerometerInformation; }

module MusicGenerator =
    let getNewMusicInformation left right =
         let vol = abs(abs(left.Speed - left.Previous.Speed) - abs(right.Speed - right.Previous.Speed))
         let baseValueLeft = decimal((left.CurrentRotation.X + left.CurrentRotation.Y + left.CurrentRotation.Z) - (left.Previous.CurrentRotation.X + left.Previous.CurrentRotation.Y + left.Previous.CurrentRotation.Z)) * 5.0m
         let baseValueRight = decimal(abs((left.CurrentRotation.X + left.CurrentRotation.Y + left.CurrentRotation.Z) - (right.Previous.CurrentRotation.X + right.Previous.CurrentRotation.Y + right.Previous.CurrentRotation.Z)))
         let result = {Treble = baseValueLeft; Tenor = baseValueRight - 32.039m; Bass = baseValueRight; Seventh = baseValueLeft - 32.039m; Volume = vol; }
         result
