<template>
  <div class="home">
    <Board :tries="tries" :word-length="wordLength" :board-state="boardState" />
    <Keyboard @clicked="updateBoard" :keyboard-state="keyboardState" />
    <Modal :class="{hide: !showModal}" @next="nextLevel" @share="shareScore" :show-next="wordLength < 10"/>
  </div>
</template>

<script>
// @ is an alias to /src
import Board from "@/components/Board.vue";
import Keyboard from "@/components/Keyboard.vue";
import Modal from "@/components/Modal.vue"
import axios from "axios";

const sleep = function (ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
};

const countLetterOccurences = function (word) {
  let result = {};
  for (let i = 0; i < word.length; i++) {
    let letter = word.charAt(i);
    result[letter] = letter in result ? result[letter] + 1 : 1;
  }

  return result;
};

const shareScore = function() {
  const vm = this;
  let result = `Shmurdle ${vm.evaluations.length} / ${vm.tries}`;

  for(let i = 0; i < vm.evaluations.length; i++) {
    result += "\n"
    for(let j = 0; j < vm.evaluations[i].length; j++) {
      switch(vm.evaluations[i][j]) {
        case "correct":
          result += "🟩";
          break;
        case "present":
          result += "🟨";
          break;
        default:
          result += "⬛";
      }
    }
  }

  navigator.share({
    title: "Share results",
    text: result
  });
}

const countOccurrences = (arr, val) =>
  arr.reduce((a, v) => (v === val ? a + 1 : a), 0);

const revertToIdle = function (row, column) {
  const vm = this;
  setTimeout(() => {
    vm.boardState[row][column].animation = "idle";
  }, 100);
};

const updateBoard = async function (data) {
  const vm = this;

  if (vm.currentRow >= vm.tries || vm.solved || vm.isLoading) {
    return;
  }

  const word = vm.boardState[vm.currentRow]
    .map(function (e) {
      return e.value;
    })
    .join("");

  if (
    vm.currentCol < vm.wordLength &&
    !["←", "↵", "Enter", "Backspace"].includes(data)
  ) {
    vm.boardState[vm.currentRow][vm.currentCol].value = data.toLowerCase();
    vm.boardState[vm.currentRow][vm.currentCol].animation = "pop";
    vm.boardState[vm.currentRow][vm.currentCol].state = "tbd";

    vm.revertToIdle(vm.currentRow, vm.currentCol);
    vm.currentCol++;
  } else if (vm.currentCol > 0 && ["←", "Backspace"].includes(data)) {
    vm.currentCol--;
    vm.boardState[vm.currentRow][vm.currentCol].value = null;
    vm.boardState[vm.currentRow][vm.currentCol].state = "empty";
  } else if (
    vm.currentRow < vm.tries &&
    ["↵", "Enter"].includes(data) &&
    !vm.boardState[vm.currentRow].some((e) => e.value === null) &&
    (await vm.isValidWord(word)) &&
    !vm.isLoading
  ) {
    vm.isLoading = true;
    let key = await vm.checkWord(vm.currentRow, word);
    vm.updateKeyboardState(key, word);
    vm.solved = countOccurrences(key, "correct") === vm.wordLength;
    vm.currentRow++;
    vm.currentCol = 0;

    vm.isLoading = false;
  }
};

const updateKeyboardState = function (key, word) {
  const vm = this;

  for (let i = 0; i < word.length; i++) {
    let letter = word.charAt(i);
    if (key[i] === "correct") {
      vm.keyboardState[letter] = "correct";
    } else if (!(letter in vm.keyboardState) && key[i] === "present") {
      vm.keyboardState[letter] = "present";
    } else if (!(letter in vm.keyboardState)) {
      vm.keyboardState[letter] = "absent";
    }
  }
};

const checkForCorrect = function (word, correctWord, data) {
  // let key = new Array(correctWord.length).fill("absent");

  for (let i = 0; i < correctWord.length; i++) {
    let letter = correctWord.charAt(i);
    if (word.charAt(i) === letter) {
      data.key[i] = "correct";
      data.lettersRemaining[letter]--;
    }
  }
};

const checkForPresent = function (word, correctWord, data) {
  for (let i = 0; i < correctWord.length; i++) {
    let letter = word.charAt(i);

    if (
      data.key[i] !== "correct" &&
      correctWord.includes(letter) &&
      data.lettersRemaining[letter]
    ) {
      data.key[i] = "present";
      data.lettersRemaining[letter]--;
    }
  }
};

const checkWord = async function (row, word) {
  const vm = this;
  let data = {
    key: new Array(vm.wordLength).fill("absent"),
    lettersRemaining: countLetterOccurences(vm.masterWord),
  };

  checkForCorrect(word, vm.masterWord, data);
  checkForPresent(word, vm.masterWord, data);

  vm.saveWordEvaluation(word, data.key);

  for (let i = 0; i < vm.wordLength; i++) {
    vm.boardState[row][i].animation = "flip-in";
    await sleep(250);

    if (data.key[i] === "correct") {
      vm.boardState[row][i].state = "correct";
    } else if (data.key[i] === "present") {
      vm.boardState[row][i].state = "present";
    } else {
      vm.boardState[row][i].state = "absent";
    }

    vm.boardState[row][i].animation = "flip-out";
    await sleep(250);
    vm.boardState[row][i].animation = "idle";
  }

  return data.key;
};

const saveWordEvaluation = async function (word, evaluation) {
  const vm = this;

  let board_state = JSON.parse(localStorage.getItem("board_state")) || [];
  let evaluations = JSON.parse(localStorage.getItem("evaluations")) || [];
  let solved = countOccurrences(evaluation, "correct") === vm.wordLength;

  if (board_state.length != evaluations.length) {
    board_state = [];
    evaluations = [];
  }

  board_state.push(word);
  evaluations.push(evaluation);
  vm.evaluations.push(evaluation);

  localStorage.setItem("board_state", JSON.stringify(board_state));
  localStorage.setItem("evaluations", JSON.stringify(evaluations));
  localStorage.setItem("solved", solved);
  localStorage.setItem("last_played", new Date().getTime());
  localStorage.setItem("word_length", vm.wordLength);
};

const resetLevel = function(wordLength) {
  const vm = this;

  if (vm.wordLength < 10) {
    vm.wordLength = wordLength;
    vm.solved = false;
    vm.keyboardState = {};
    vm.currentRow = 0;
    vm.evaluations = [];
    vm.boardState = createBoard(vm.wordLength, tries);
    localStorage.setItem("board_state", JSON.stringify([]));
    localStorage.setItem("evaluations", JSON.stringify([]));
    localStorage.setItem("solved", false);
    localStorage.setItem("last_played", new Date().getTime());
    localStorage.setItem("word_length", wordLength);
     axios
      .get(
        `/api/word-of-the-day?length=${vm.wordLength}`
        // `https://localhost:44386/api/word-of-the-day?length=${vm.wordLength}`
      )
      .then((response) => {
        vm.masterWord = response.data.word;
      });
  }
}

const loadLocalStorage = async function () {
  const vm = this;
  let last_played = localStorage.getItem("last_played") || null;

  if (
    !last_played ||
    new Date(parseInt(last_played)).getUTCDay() !== new Date().getUTCDay()
  ) {
    vm.resetLevel(5);
    return;
  }

  let board_state = JSON.parse(localStorage.getItem("board_state")) || [];
  vm.evaluations = JSON.parse(localStorage.getItem("evaluations")) || [];
  vm.solved = (localStorage.getItem("solved") || false) === "true";
  vm.wordLength = parseInt(localStorage.getItem("word_length")) || 5;
  vm.boardState = createBoard(vm.wordLength, tries);

  if (!board_state.length || !vm.evaluations.length) {
    return;
  }

  for (let i = 0; i < board_state.length; i++) {
    for (let j = 0; j < board_state[i].length; j++) {
      vm.boardState[i][j].value = board_state[i][j];
      vm.boardState[i][j].state = vm.evaluations[i][j];
    }
  }

  for (let i = 0; i < vm.wordLength; i++) {
    for (let j = 0; j < board_state.length; j++) {
      setTimeout(async () => {
        vm.boardState[j][i].animation = "flip-in";
        await sleep(250);
        vm.boardState[j][i].animation = "flip-out";
        await sleep(250);
        vm.boardState[j][i].animation = "idle";
      });
    }
    await sleep(100);
  }

  for (let i = 0; i < board_state.length; i++) {
    vm.updateKeyboardState(vm.evaluations[i], board_state[i]);
  }

  vm.currentRow = board_state.length;
};

const isValidWord = async function (word) {
  return (
    (await axios.get(`/api/check-word?word=${word}`))
    // (await axios.get(`https://localhost:44386/api/check-word?word=${word}`))
      .data
  );
};

const createBoard = function (wordLength, tries) {
  let board = new Array(tries);

  for (let i = 0; i < tries; i++) {
    board[i] = new Array(wordLength);
    for (let j = 0; j < wordLength; j++) {
      board[i][j] = {
        value: null,
        state: "empty",
        animation: "idle",
      };
    }
  }

  return board;
};

const nextLevel = function() {
  const vm = this;
  vm.resetLevel(vm.wordLength + 1);
  vm.showModal = false;
}

const tries = 6;
const wordLength = 5;

export default {
  name: "Home",
  data() {
    return {
      currentRow: 0,
      currentCol: 0,
      tries: tries,
      wordLength: wordLength,
      boardState: createBoard(wordLength, tries),
      masterWord: null,
      solved: false,
      keyboardState: {},
      isLoading: false,
      showModal: false,
      evaluation: []
    };
  },
  components: {
    Board,
    Keyboard,
    Modal
  },
  methods: {
    updateBoard: updateBoard,
    isValidWord: isValidWord,
    revertToIdle: revertToIdle,
    checkWord: checkWord,
    updateKeyboardState: updateKeyboardState,
    saveWordEvaluation: saveWordEvaluation,
    loadLocalStorage: loadLocalStorage,
    resetLevel: resetLevel,
    shareScore: shareScore,
    nextLevel: nextLevel
  },
  watch: {
    solved: function() {
      const vm = this;
      if(vm.solved) {
        vm.showModal = true;
      }
    }
  },
  async mounted() {
    const vm = this;
    window.addEventListener("keydown", function (e) {
      if (
        /^[a-zA-Z]+$/.test(e.key) &&
        (e.key.length === 1 || ["Enter", "Backspace"].includes(e.key))
      ) {
        vm.updateBoard(e.key);
      }
    });

    await vm.loadLocalStorage();

    axios
      .get(
        `/api/word-of-the-day?length=${vm.wordLength}`
        // `https://localhost:44386/api/word-of-the-day?length=${vm.wordLength}`
      )
      .then((response) => {
        vm.masterWord = response.data.word;
      });

  },
};
</script>

<style scoped lang="scss">
.home {
  width: 100%;
  margin: 0 auto;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.hide {
  display: none;
}
</style>
