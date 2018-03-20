namespace FSharpGame

open Godot

module Icon =
    type Class() =
        inherit Sprite()
        override this._Ready () = GD.Print("FSharp is ready")
        override this._Process (delta) =
            let pos = this.GetPosition()
            let increment = Vector2(50.0f * delta, 0.0f)
            if Input.IsActionPressed("key_left") then
                this.SetPosition(pos - increment)
            if Input.IsActionPressed("key_right") then
                this.SetPosition(pos + increment)