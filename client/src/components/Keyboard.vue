<template>
  <div class="keyboard">
    <div v-for="row in keys" :key="row" class="row">
      <button
        v-for="key in row"
        :key="key"
        :class="key.type"
        @click="clicked(key.data)"
        :disabled="key.type == 'half'"
        :data-state="
          keyboardState[key.letter] ? keyboardState[key.letter] : 'empty'
        "
      >
        {{ key.letter }}<span v-if="key.html" v-html="key.html"></span>
      </button>
    </div>
  </div>
</template>

<script>
const clicked = function (letter) {
  const vm = this;
  vm.$emit("clicked", letter);
};

export default {
  name: "Keyboard",
  props: {
    keyboardState: Object,
  },
  data() {
    return {
      keys: [
        [
          {
            letter: "q",
            data: "q",
            type: "full",
          },
          {
            letter: "w",
            data: "w",
            type: "full",
          },
          {
            letter: "e",
            data: "e",
            type: "full",
          },
          {
            letter: "r",
            data: "r",
            type: "full",
          },
          {
            letter: "t",
            data: "t",
            type: "full",
          },
          {
            letter: "y",
            data: "y",
            type: "full",
          },
          {
            letter: "u",
            data: "u",
            type: "full",
          },
          {
            letter: "i",
            data: "i",
            type: "full",
          },
          {
            letter: "o",
            data: "o",
            type: "full",
          },
          {
            letter: "p",
            data: "p",
            type: "full",
          },
        ],
        [
          {
            letter: "",
            type: "half",
          },
          {
            letter: "a",
            data: "a",
            type: "full",
          },
          {
            letter: "s",
            data: "s",
            type: "full",
          },
          {
            letter: "d",
            data: "d",
            type: "full",
          },
          {
            letter: "f",
            data: "f",
            type: "full",
          },
          {
            letter: "g",
            data: "g",
            type: "full",
          },
          {
            letter: "h",
            data: "h",
            type: "full",
          },
          {
            letter: "j",
            data: "j",
            type: "full",
          },
          {
            letter: "k",
            data: "k",
            type: "full",
          },
          {
            letter: "l",
            data: "l",
            type: "full",
          },
          {
            letter: "",
            type: "half",
          },
        ],
        [
          {
            letter: "Enter",
            data: "↵",
            type: "full-half",
          },
          {
            letter: "z",
            data: "z",
            type: "full",
          },
          {
            letter: "x",
            data: "x",
            type: "full",
          },
          {
            letter: "c",
            data: "c",
            type: "full",
          },
          {
            letter: "v",
            data: "v",
            type: "full",
          },
          {
            letter: "b",
            data: "b",
            type: "full",
          },
          {
            letter: "n",
            data: "n",
            type: "full",
          },
          {
            letter: "m",
            data: "m",
            type: "full",
          },
          {
            letter: "",
            data: "←",
            type: "full-half",
            html: `
                <svg xmlns="http://www.w3.org/2000/svg" height="24" viewBox="0 0 24 24" width="24">
                    <path fill="var(--color-tone-1)" d="M22 3H7c-.69 0-1.23.35-1.59.88L0 12l5.41 8.11c.36.53.9.89 1.59.89h15c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H7.07L2.4 12l4.66-7H22v14zm-11.59-2L14 13.41 17.59 17 19 15.59 15.41 12 19 8.41 17.59 7 14 10.59 10.41 7 9 8.41 12.59 12 9 15.59z"></path>
                </svg>
                `,
          },
        ],
      ],
    };
  },
  methods: {
    clicked: clicked,
  },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
.keyboard {
  width: 100%;
  margin: 0 auto;
  max-width: 500px;
  user-select: none;
}

.row {
  display: flex;
  width: 100%;
  margin: 0 auto 8px;
  touch-action: manipulation;
}

button {
  font-family: inherit;
  font-weight: bold;
  border: 0;
  padding: 0;
  margin: 0 6px 0 0;
  height: 58px;
  border-radius: 4px;
  cursor: pointer;
  user-select: none;
  background-color: #d3d6da;
  color: black;
  flex: 1;
  display: flex;
  justify-content: center;
  align-items: center;
  text-transform: uppercase;
}

button.half {
  flex: 0.5;
  background: none;
  cursor: initial;
}

button.full-half {
  flex: 1.5;
}

button[data-state="correct"] {
  transition: background-color 0.1s ease, color 0.1s ease;
  background-color: #6aaa64;
}

button[data-state="present"] {
  transition: background-color 0.1s ease, color 0.1s ease;
  background-color: #c9b458;
}

button[data-state="absent"] {
  transition: background-color 0.1s ease, color 0.1s ease;
  background-color: #787c7e;
}
</style>
