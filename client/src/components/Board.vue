<template>
  <div class="container">
    <div class="board">
      <div v-for="(i, row) in tries" class="row" :key="row">
        <div
          v-for="(j, col) in wordLength"
          class="tile"
          :data-state="boardState[row][col].state"
          :data-animation="boardState[row][col].animation"
          :key="col"
        >
          {{ boardState[row][col].value }}
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "Board",
  props: {
    tries: Number,
    wordLength: Number,
    boardState: Array,
  }
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
.container {
  display: flex;
  justify-content: center;
  align-items: center;
  flex-grow: 1;
  overflow: hidden;
}

.board {
  display: grid;
  grid-gap: 5px;
  padding: 10px;
  box-sizing: border-box;
}

.row {
  display: flex;
  grid-gap: 5px;
}

.tile {
  width: 100%;
  display: inline-flex;
  justify-content: center;
  align-items: center;
  font-size: 2rem;
  line-height: 2rem;
  font-weight: bold;
  vertical-align: middle;
  box-sizing: border-box;
  color: white;
  text-transform: uppercase;
  user-select: none;
  width: 62px;
}

.tile[data-state="tbd"] {
  border: 2px solid #878a8c;
  color: black;
}

.tile[data-state="empty"] {
  border: 2px solid #d3d6da;
}

.tile::before {
  content: "";
  display: inline-block;
  padding-bottom: 100%;
}

.tile[data-animation="pop"] {
  animation-name: PopIn;
  animation-duration: 100ms;
}

.tile[data-animation="flip-in"] {
  animation-name: FlipIn;
  animation-duration: 250ms;
  animation-timing-function: ease-in;
}

.tile[data-animation="flip-out"] {
  animation-name: FlipOut;
  animation-duration: 250ms;
  animation-timing-function: ease-in;
}

.tile[data-state="correct"] {
  background-color: #6aaa64;
}

.tile[data-state="present"] {
  background-color: #c9b458;
}

.tile[data-state="absent"] {
  background-color: #787c7e;
}

@keyframes PopIn {
  from {
    transform: scale(0.8);
    opacity: 0;
  }

  40% {
    transform: scale(1.1);
    opacity: 1;
  }
}

@keyframes FlipIn {
  0% {
    transform: rotateX(0);
  }
  100% {
    transform: rotateX(-90deg);
  }
}

@keyframes FlipOut {
  0% {
    transform: rotateX(-90deg);
  }
  100% {
    transform: rotateX(0);
  }
}
</style>
