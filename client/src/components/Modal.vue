<script>
export default {
  name: "Modal",
  props: {
    showNext: Boolean,
  },
  data() {
    return {
        time: ""
    };
  },
  methods: {
    share() {
      this.$emit("share");
    },
    next() {
      this.$emit("next");
    },
    updateTime() {
      const vm = this;
      let currentTime = new Date(Date.now());

      let hours = `${23 - currentTime.getUTCHours()}`;
      let minutes = `${60 - currentTime.getUTCMinutes()}`;
      let seconds = `${60 - currentTime.getUTCSeconds()}`;

      vm.time = 
        `${hours.padStart(2, 0)}:${minutes.padStart(2, 0)}:${seconds.padStart(
          2,
          0
        )}`;
    },
  },
  mounted: function () {
    const vm = this;
    window.setInterval(() => {
      vm.updateTime();
    }, 1000);
  },
};
</script>

<template>
  <transition name="modal-fade">
    <div class="modal-backdrop">
      <div
        class="modal"
        role="dialog"
        aria-labelledby="modalTitle"
        aria-describedby="modalDescription"
      >
        <!-- <header
          class="modal-header"
          id="modalTitle"
        >
          <slot name="header">
            This is the default tile!
          </slot>
          <button
            type="button"
            class="btn-close"
            @click="close"
            aria-label="Close modal"
          >
            x
          </button>
        </header> -->

        <section class="modal-body" id="modalDescription">
          <slot name="body">
            Time until next shmurdle: {{time}}
          </slot>
        </section>

        <footer class="modal-footer">
          <button @click="share">Share</button>
          <button v-show="showNext" @click="next">Next</button>
        </footer>
      </div>
    </div>
  </transition>
</template>

<style>
.modal-backdrop {
  position: fixed;
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
  background-color: rgba(0, 0, 0, 0.3);
  display: flex;
  justify-content: center;
  align-items: center;
}

.modal {
  background: #ffffff;
  box-shadow: 2px 2px 20px 1px;
  overflow-x: auto;
  display: flex;
  flex-direction: column;
}

.modal-header,
.modal-footer {
  padding: 15px;
  display: flex;
}

.modal-header {
  position: relative;
  border-bottom: 1px solid #eeeeee;
  color: #4aae9b;
  justify-content: space-between;
}

.modal-footer {
  border-top: 1px solid #eeeeee;
  flex-direction: column;
}

.modal-body {
  position: relative;
  padding: 20px 10px;
}

.btn-close {
  position: absolute;
  top: 0;
  right: 0;
  border: none;
  font-size: 20px;
  padding: 10px;
  cursor: pointer;
  font-weight: bold;
  color: #4aae9b;
  background: transparent;
}

.btn-green {
  color: white;
  background: #4aae9b;
  border: 1px solid #4aae9b;
  border-radius: 2px;
}

.modal-fade-enter,
.modal-fade-leave-to {
  opacity: 0;
}

.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: opacity 0.5s ease;
}
</style>
