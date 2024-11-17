<template>
    <div class="container">
        <div class="container-fluid text-center mt-5 mb-5 pt-4 pb-4 bg-light-subtle border border-light-subtle rounded-3">
            <div v-if="this.$root.user === null">
                <div class="row">
                    <h1> Login Page </h1>
                </div>
                <div class="row">
                    <p> This is where you fill in your credentials... </p>
                </div>
            </div>
            <div v-else>
                <div class="row">
                    <h1> Panel Page </h1>
                </div>
                <div class="row">
                    <p> Hi, {{ this.$root.user }}. This is the danger zone. </p>
                </div>
            </div>
        </div>
        
        <login-form v-if="this.$root.user === null"></login-form>
        <control-panel v-else></control-panel>
    </div>
</template>

<script>
import LoginForm from './LoginForm.vue'
import ControlPanel from './ControlPanel.vue'

export default {
    components: {
        LoginForm, ControlPanel
    }
}
</script>