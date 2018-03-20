namespace FSharpGame

open Godot

module IconFp =
    // All pure code is outside the class. Everything is immutable and there are
    // no side effects. This has the main game logic.

    type State = { 
        velocity: Vector2
        acceleration : float32
        drag : float32
        minXVelocity : float32
    }

    type Input = {
        isLeftKeyPressed: bool
        isRightKeyPressed: bool
        position: Vector2
    }

    type Output = {
        position: Vector2
    }

    let initialState : State = {
        velocity = Vector2(0.0f, 0.0f)
        acceleration = 70.0f
        drag = 30.0f
        minXVelocity = 10.0f
    }

    let applyXAcceleration acceleration state delta =
        { state with velocity = state.velocity + Vector2(acceleration * delta, 0.0f) }
    
    let applyDrag state delta =
        if abs state.velocity.x < state.minXVelocity then
            { state with velocity = Vector2(0.0f, 0.0f) }
        else
            applyXAcceleration 
                (state.drag * float32 (sign state.velocity.x) * -1.0f)
                state
                delta

    let stateToOutput (input: Input) state delta =
        { position = input.position + (state.velocity * delta) }

    let processFrame (input: Input) (state: State) (delta: float32) =
        let newState =
            if input.isLeftKeyPressed then
                applyXAcceleration (state.acceleration * -1.0f) state delta
            else if input.isRightKeyPressed then
                applyXAcceleration state.acceleration state delta
            else
                applyDrag state delta
        (newState, stateToOutput input newState delta)

    // All impure code goes inside the class. The class only has plumbing for 
    // the side effects and for keeping state between frames.
    type Class() =
        inherit Sprite()
        let mutable state : State = initialState
        override this._Process delta =
            let (newState, output) =
                processFrame
                    { isLeftKeyPressed = Input.IsActionPressed("key_left")
                      isRightKeyPressed = Input.IsActionPressed("key_right")
                      position = this.GetPosition()
                    }
                    state
                    delta
            state <- newState
            this.SetPosition(output.position)