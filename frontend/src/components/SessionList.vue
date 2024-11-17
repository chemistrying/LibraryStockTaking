<template>
    <div class="container text-center">
        <h3> Current Acitve Session: </h3>
    </div>
    <div class="container">
        <div class="row row-cols-1 row-cols-lg-3 justify-content-lg-center g-4">
            <div class="col" v-for="session in this.$root.allSessionsInfo">
                <session-card :session="session"></session-card>
            </div>
        </div>
    </div>
</template>

<script>
import SessionCard from './SessionCard.vue';

export default {
    components: {
        SessionCard
    },
    computed: {
        async fetchAllSessions() {
            this.$root.allSessionsInfo = await fetch(`${this.$root.apiUrl}/api/sessions`).then(response => response.json());
        }
    },
    data() {
        return {
            cardCount: 0,
            allSessionsInfo: [],
            isAdmin: false
        }
    },
    async mounted() {
        if (this.$root.isAdmin) {
            this.$root.allSessionsInfo = await fetch(`${this.$root.apiUrl}/api/sessions`).then(response => response.json());
        } else {
            this.$root.allSessionsInfo = [await fetch(`${this.$root.apiUrl}/api/active`).then(response => response.json())];
        }
    }
}
</script>