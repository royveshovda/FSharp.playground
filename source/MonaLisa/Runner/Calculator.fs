module Calculator

open System

type Point =
        struct
            val X:int
            val Y:int
            new (x:int,y:int) = { X=x; Y=y }
        end

type Solution = Point []

//type Evaluation =
//    struct
//        val Value:float
//        val Solution:Solution
//        new (value:float,solution:Solution) = { Value=value; Solution=solution }
//    end

let dist (p1:Point) (p2:Point) =
    (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)

// Length: the correct distance should be the sqare root of the solution 

let length (xs:Solution) =
    let len = xs |> Array.length
    xs
    |> Seq.fold (fun (acc,prev) x ->
        acc + dist x  xs.[prev], (prev+1)%len) (0,len-1)
    |> fst

//let quality (xs:Solution) = - length xs

let shuffle (rng:Random) (xs) =
    let len = xs |> Array.length
    for i in (len - 1) .. -1 .. 1 do
        let j = rng.Next(i + 1)
        let temp = xs.[j]
        xs.[j] <- xs.[i]
        xs.[i] <- temp
    xs

let clone (solution:Solution) : Solution =
    Array.copy solution

let best solution1 solution2 =
    let length1 = length solution1
    let length2 = length solution2

    match length1 <= length2 with
    | true -> solution1
    | false -> solution2